using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct PathPlanning : ISystem
{
    private GameConfig _config;
    private bool _initialized;
    private DynamicBuffer<GridCell> _grid;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameConfig>();
        state.RequireForUpdate<GridCell>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!_initialized)
        {
            _initialized = true;
            _config = SystemAPI.GetSingleton<GameConfig>();

            var gameConfigEntity = SystemAPI.GetSingletonEntity<GameConfig>();
            
            _grid = state.EntityManager.GetBuffer<GridCell>(gameConfigEntity);
        }

        // new PathFindingJob()
        // {
        //     //grid = _grid,
        //     gridSize = _config.mazeSize,
        //     startPos = new int2(5,5),
        //     destinationPos = new int2(10,10)
        // }.ScheduleParallel();
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
    //public DynamicBuffer<GridCell> grid;
    public int gridSize;
    public int2 startPos;
    public int2 destinationPos;
    
    public void Execute(DynamicBuffer<Trajectory> trajectory)
    {
        var cellArray = new NativeArray<PathCell>(gridSize * gridSize, Allocator.Temp);
        
        //Initialize gridArray
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                PathCell cell = new PathCell
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

        NativeArray<int2> neighborOffsetArray = new NativeArray<int2>(new[]
        {
            new int2(-1, 0), // Left
            new int2(1, 0), // Right
            new int2(0, -1), // Up
            new int2(0, 1), // Down
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
            int currentCellIndex = GetLowestFScoreIndex(openList, cellArray);
            PathCell currentCell = cellArray[currentCellIndex];

            if (currentCellIndex == destinationCellIndex)
            {
                // Reached destination
                break;
            }
            
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

                var neighborCell = cellArray[neighborIndex];

                int2 currentCellPosition = new int2(currentCell.x, currentCell.y);

                int tentativeGCost = currentCell.gCost + CalculateHCost(currentCellPosition, neighborPosition);
                if (tentativeGCost < neighborCell.gCost)
                {
                    neighborCell.cameFrom = currentCellIndex;
                    neighborCell.gCost = tentativeGCost;
                    neighborCell.CalculateFCost();
                    cellArray[neighborIndex] = neighborCell;
                    
                    if(!openList.Contains(neighborIndex))
                        openList.Add(neighborIndex);
                }
            }
        }

        var destinationCell = cellArray[destinationCellIndex];
        if (destinationCell.cameFrom == -1)
        {
            
        }
        else
        {
            // //Calculate path
            // NativeList<int> path = new NativeList<int>(Allocator.Temp);
            // path.Add(destinationCell.index);
            //
            // var currentCell = destinationCell;
            // while (currentCell.cameFrom!= -1)
            // {
            //     var cameFromCell = cellArray[currentCell.cameFrom];
            //     path.Add(cameFromCell.index);
            //     currentCell = cameFromCell;
            // }
            //
            // trajectory.Clear();
            //
            // for (int i = 0; i < path.Length; i++)
            // {
            //     trajectory.Add(path[i]);
            // }
        }

        cellArray.Dispose();
        openList.Dispose();
        closedList.Dispose();
        neighborOffsetArray.Dispose();
    }

    private bool InBounds(int2 pos, int gridSize)
    {
        return pos.x >= 0 && pos.x < gridSize && pos.y >= 0 && pos.y < gridSize;
    }

    private int GetLowestFScoreIndex(NativeList<int> openList, NativeArray<PathCell> cellArray)
    {
        var lowestCost = cellArray[0];
        for (int i = 0; i < openList.Length; i++)
        {
            var testCell = cellArray[i];
            if (testCell.fCost < lowestCost.fCost)
            {
                lowestCost = testCell;
            }
        }

        return lowestCost.index;
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
