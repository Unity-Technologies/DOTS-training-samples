using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct PathPlanning : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameConfig>();
        state.RequireForUpdate<GridCell>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<GameConfig>();

        var gameConfigEntity = SystemAPI.GetSingletonEntity<GameConfig>();

        var grid = state.EntityManager.GetBuffer<GridCell>(gameConfigEntity);
        
        var gridArray = new NativeArray<byte>(grid.Length, Allocator.TempJob);

        for (int i = 0; i < grid.Length; i++)
        {
            gridArray[i] = grid[i].wallFlags;
        }

        var jobHandle = new PathFindingJob()
        {
            grid = gridArray,
            gridSize = config.mazeSize,
            startPos = new int2(0, 5),
            destinationPos = new int2(15, 7)
        }.ScheduleParallel(state.Dependency);
        
        jobHandle.Complete();
        gridArray.Dispose();

        var query = state.EntityManager.CreateEntityQuery(typeof(NeedUpdateTrajectory));
        
        foreach (var entity in query.ToEntityArray(Allocator.Temp))
        {
            state.EntityManager.SetComponentEnabled<NeedUpdateTrajectory>(entity, false);
        }
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}

[WithAll(typeof(NeedUpdateTrajectory))]
[BurstCompile]
public partial struct PathFindingJob : IJobEntity
{
    [ReadOnly]
    public NativeArray<byte> grid;
    public int gridSize;
    public int2 startPos;
    public int2 destinationPos;
    
    public void Execute(DynamicBuffer<Trajectory> trajectory)
    {
        var cellArray = new NativeArray<PathCell>(gridSize * gridSize, Allocator.Temp);
        
        //Initialize cellArray
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                var cell = new PathCell
                {
                    x = x,
                    y = y,
                    index = x + y * gridSize,
                    gCost = int.MaxValue,
                    hCost = CalculateHCost(new int2(x, y), destinationPos),
                    cameFrom = -1
                };
                cell.CalculateFCost();

                cellArray[cell.index] = cell;
            }
        }

        var neighborOffsetArray = new NativeArray<int2>(new[]
        {
            new int2(0, -1), // up
            new int2(0, 1), // down
            new int2(1, 0), // right
            new int2(-1, 0), // left
        }, Allocator.Temp);

        int destinationCellIndex = cellArray[destinationPos.x + destinationPos.y * gridSize].index;
        
        var startCell = cellArray[startPos.x + startPos.y * gridSize];
        startCell.gCost = 0;
        startCell.CalculateFCost();
        cellArray[startCell.index] = startCell;

        var openList = new NativeList<int>(Allocator.Temp);
        var closedList = new NativeList<int>(Allocator.Temp);
        
        openList.Add(startCell.index);

        while (openList.Length > 0)
        {
            // Get lowest fScore
            var lowestCost = cellArray[openList[0]];
            for (int i = 0; i < openList.Length; i++)
            {
                var testCell = cellArray[openList[i]];
                if (testCell.fCost < lowestCost.fCost)
                    lowestCost = testCell;
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
                
                int neighborIndex = cellArray[neighborPosition.x + neighborPosition.y * gridSize].index;
                if (closedList.Contains(neighborIndex))
                    continue;

                //Check walls
                switch (i)
                {
                    case 0: //up
                        if ((grid[currentCellIndex] & (1 << 1)) != 0)
                            continue;
                        break;
                    case 1: //down
                        if ((grid[currentCellIndex] & (1 << 0)) != 0)
                            continue;
                        break;
                    case 2: //right
                        if ((grid[currentCellIndex] & (1 << 2)) != 0)
                            continue;
                        break;
                    case 3: //left
                        if ((grid[currentCellIndex] & (1 << 3)) != 0)
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

                if(!openList.Contains(neighborIndex))
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

            path.Dispose();
        }

        cellArray.Dispose();
        openList.Dispose();
        closedList.Dispose();
        neighborOffsetArray.Dispose();
    }

    private bool InBounds(int2 pos, int size)
    {
        return pos.x >= 0 && pos.x < size && pos.y >= 0 && pos.y < size;
    }

    private int CalculateHCost(int2 start, int2 end)
    {
        int dx = math.abs(start.x - end.x);
        int dy = math.abs(start.y - end.y);
        return 10 * (dx + dy);
    }
}

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