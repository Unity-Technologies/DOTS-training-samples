using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Profiling;
using Unity.Transforms;

[BurstCompile]
public partial struct PathPlanning : ISystem
{
    private NativeArray<int2> neighborOffsetArray;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameConfig>();
        state.RequireForUpdate<GridCell>();
        state.RequireForUpdate<HunterTarget>();

        neighborOffsetArray = new NativeArray<int2>(4, Allocator.Persistent);
        neighborOffsetArray[0] = new int2(0, -1);
        neighborOffsetArray[1] = new int2(0, 1);
        neighborOffsetArray[2] = new int2(1, 0);
        neighborOffsetArray[3] = new int2(-1, 0);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<GameConfig>();

        var gameConfigEntity = SystemAPI.GetSingletonEntity<GameConfig>();

        var grid = state.EntityManager.GetBuffer<GridCell>(gameConfigEntity);

        var transformFromEntity = SystemAPI.GetComponentLookup<LocalToWorldTransform>(true);
        
        // Get an EntityCommandBuffer from the BeginSimulationEntityCommandBufferSystem.
        var ecbSingleton = SystemAPI.GetSingleton<
            BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        new PathFindingJob()
        {
            neighborOffsetArray = neighborOffsetArray,
            Ecb = ecb.AsParallelWriter(),
            targetTransform = transformFromEntity,
            grid = grid,
            gridSize = config.mazeSize,
        }.ScheduleParallel();
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        neighborOffsetArray.Dispose();
    }
}

[WithAll(typeof(NeedUpdateTrajectory))]
[WithAll(typeof(HunterTarget))]
[WithAll(typeof(PositionInPath))]
[BurstCompile]
public partial struct PathFindingJob : IJobEntity
{
    [ReadOnly] public NativeArray<int2> neighborOffsetArray;
    [ReadOnly] public DynamicBuffer<GridCell> grid;
    [ReadOnly] public ComponentLookup<LocalToWorldTransform> targetTransform;
    public EntityCommandBuffer.ParallelWriter Ecb;
    public int gridSize;
    private ProfilerMarker JobMarker;

    [BurstCompile]
    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, DynamicBuffer<Trajectory> trajectory, HunterTarget target, ref PositionInPath positionInPath)
    {
        JobMarker = new ProfilerMarker("PathfindingJob");
        JobMarker.Begin();
        
        var targetPosition = target.position;
        var unitPosition = targetTransform[entity].Value.Position;
        
        int2 startPos = MazeUtils.WorldPositionToGrid(unitPosition);
        int2 destinationPos = MazeUtils.WorldPositionToGrid(targetPosition);

        var cellArray = new NativeParallelHashMap<int, PathCell>(128, Allocator.Temp);

        int destinationCellIndex = destinationPos.x + destinationPos.y * gridSize;
        cellArray.Add(destinationCellIndex, CreateNewCell(destinationPos,destinationCellIndex, startPos, destinationPos));
        
        var startCell = CreateNewCell(startPos, startPos.x + startPos.y * gridSize, startPos, destinationPos);
        startCell.gCost = 0;
        startCell.CalculateFCost();
        cellArray.TryAdd(startCell.index, startCell);

        var openList = new NativeList<int>(Allocator.Temp);
        var closedList = new NativeList<int>(Allocator.Temp);
        
        openList.Add(startCell.index);

        while (openList.Length > 0)
        {
            // Get lowest fScore
            var lowestCost = cellArray[openList[0]];
            for (int i = 0; i < openList.Length; i++)
            {
                if (cellArray[openList[i]].fCost < lowestCost.fCost)
                    lowestCost = cellArray[openList[i]];
            }

            int currentCellIndex = lowestCost.index;

            PathCell currentCell = cellArray[currentCellIndex];

            if (currentCellIndex == destinationCellIndex)
                break;
            
            //Remove current node from OpenList
            for (int i = 0; i < openList.Length; i++)
            {
                if (openList[i] == currentCellIndex)
                {
                    openList.RemoveAtSwapBack(i);
                    break;
                }
            }
            
            closedList.Add(currentCellIndex);

            for (int i = 0; i < neighborOffsetArray.Length; i++)
            {
                int2 neighborOffset = neighborOffsetArray[i];
                int2 neighborPosition = new int2(currentCell.x + neighborOffset.x, currentCell.y + neighborOffset.y);

                if (!InBounds(neighborPosition, gridSize))
                    continue;
                
                int neighborIndex = neighborPosition.x + neighborPosition.y * gridSize;
                if (cellArray.TryGetValue(neighborIndex, out PathCell cell))
                {
                    neighborIndex = cell.index;   
                }
                else
                {
                    PathCell newNeighborCell = CreateNewCell(neighborPosition, neighborIndex, startPos, destinationPos);
                    cellArray.Add(neighborIndex, newNeighborCell);
                }

                bool inClosedList = false;
                for (int j = 0; j < closedList.Length; j++)
                {
                    if (closedList[j] == neighborIndex)
                    {
                        inClosedList = true;
                        break;
                    }
                }
                
                if (inClosedList)
                    continue;

                //Check walls
                switch (i)
                {
                    case 0: //up
                        if ((grid[currentCellIndex].wallFlags & (1 << 1)) != 0)
                            continue;
                        break;
                    case 1: //down
                        if ((grid[currentCellIndex].wallFlags & (1 << 0)) != 0)
                            continue;
                        break;
                    case 2: //right
                        if ((grid[currentCellIndex].wallFlags & (1 << 2)) != 0)
                            continue;
                        break;
                    case 3: //left
                        if ((grid[currentCellIndex].wallFlags & (1 << 3)) != 0)
                            continue;
                        break;
                }

                var neighborCell = cellArray[neighborIndex];

                int2 currentCellPosition = new int2(currentCell.x, currentCell.y);

                int tentativeGCost = currentCell.gCost + CalculateHCost(currentCellPosition, neighborPosition);
                if (tentativeGCost >= neighborCell.gCost) 
                    continue;
                
                neighborCell.cameFrom = currentCellIndex;
                neighborCell.gCost = tentativeGCost;
                neighborCell.CalculateFCost();
                cellArray[neighborIndex] = neighborCell;

                bool inOpenList = false;

                for (int j = 0; j < openList.Length; j++)
                {
                    if (openList[j] == neighborIndex)
                        inOpenList = true;
                }
                
                if(!inOpenList)
                    openList.Add(neighborIndex);
            }
        }

        var destinationCell = cellArray[destinationCellIndex];
        if (destinationCell.cameFrom == -1)
        {
            // no path found
        }
        else
        {
            //Calculate path
            NativeList<int> path = new NativeList<int>(Allocator.Temp);
            path.Add(destinationCell.index);
            
            var currentCell = destinationCell;
            while (currentCell.cameFrom!= -1)
            {
                var cameFromCell = cellArray[currentCell.cameFrom];
                path.Add(cameFromCell.index);
                currentCell = cameFromCell;
            }
            
            trajectory.Clear();
            
            for (int i = path.Length-1; i >= 0; i--)
            {
                trajectory.Add(path[i]);
            }
            positionInPath.currentIndex = 0;
            path.Dispose();
        }

        cellArray.Dispose();
        openList.Dispose();
        closedList.Dispose();
        
        JobMarker.End();
        
        Ecb.SetComponentEnabled<NeedUpdateTrajectory>(chunkIndex, entity, false);
    }

    private PathCell CreateNewCell(int2 gridPos, int index, int2 start, int2 end)
    {
        PathCell cell = new PathCell
        {
            x = gridPos.x,
            y = gridPos.y,
            index = index,
            gCost = int.MaxValue,
            cameFrom = -1,
            hCost = CalculateHCost(new int2(gridPos.x, gridPos.y), end)
        };
        cell.CalculateFCost();
        return cell;
    }

    [BurstCompile]
    private bool InBounds(int2 pos, int size)
    {
        return pos.x >= 0 && pos.x < size && pos.y >= 0 && pos.y < size;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CalculateHCost(int2 start, int2 end)
    {
        int dx = math.abs(start.x - end.x);
        int dy = math.abs(start.y - end.y);
        return 10 * (dx + dy);
    }
}

[BurstCompile]
public struct PathCell
{
    public int x;
    public int y;
    public int index;
    public int gCost;
    public int hCost;
    public int fCost;
    public int cameFrom;

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
}
