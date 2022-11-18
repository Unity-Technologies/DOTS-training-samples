using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;

using Random = Unity.Mathematics.Random;

[BurstCompile]
struct CreateMazeFreeZonesJob : IJobParallelFor
{
    [NativeDisableContainerSafetyRestriction]
    public DynamicBuffer<GridCell> grid;
    public int stride;
    public int stripWidth;
    [ReadOnly]
    public NativeArray<int> pathCenters;
    public int halfPathWidth;

    public void Execute(int index)
    {
        int y = index / stride;
        int x = index % stride;

        if (!((x > stripWidth && y > stripWidth) && (x < stride - stripWidth && y < stride - stripWidth)))
        {
            // outside the maze area
            var tmp = grid[MazeUtils.CellIdxFromPos(x, y, stride)];
            tmp.wallFlags = (byte)WallFlags.None;
            grid[MazeUtils.CellIdxFromPos(x, y, stride)] = tmp;
        }

        for (int i = 0; i < pathCenters.Length; i++)
        {
            if (x > stripWidth + (pathCenters[i] - halfPathWidth) && y > stripWidth && x < stripWidth + (pathCenters[i] + halfPathWidth) && y < stride - stripWidth)
            {
                // outside the maze area
                var tmp = grid[MazeUtils.CellIdxFromPos(x, y, stride)];
                tmp.wallFlags = (byte)WallFlags.None;
                grid[MazeUtils.CellIdxFromPos(x, y, stride)] = tmp;
            }
        }
    }
}

[BurstCompile]
struct SpawnWalls : IJobParallelFor
{
    [ReadOnly]
    public DynamicBuffer<GridCell> grid;
    public EntityCommandBuffer.ParallelWriter ecb;
    public Entity wallPrefab;
    public int stride;

    public void Execute(int index)
    {
        int x = index % stride;
        int y = index / stride;

        var cellFlags = (WallFlags)grid[index].wallFlags;
        if(MazeUtils.HasFlag(cellFlags, WallFlags.West))
        {
            var wall = ecb.Instantiate(index, wallPrefab);
            ecb.SetComponent(index, wall, new LocalToWorldTransform
            {
                Value = UniformScaleTransform.FromPositionRotation(MazeUtils.GridPositionToWorld(x, y) + new float3(-0.5f, 0, 0), quaternion.AxisAngle(math.up(), math.radians(270)))
            });
        }
        if(x == stride - 1 && MazeUtils.HasFlag(cellFlags, WallFlags.East))
        {
            var wall = ecb.Instantiate(index, wallPrefab);
            ecb.SetComponent(index, wall, new LocalToWorldTransform
            {
                Value = UniformScaleTransform.FromPositionRotation(MazeUtils.GridPositionToWorld(x, y) + new float3(0.5f, 0, 0), quaternion.AxisAngle(math.up(), math.radians(90)))
            });
        }
        if(MazeUtils.HasFlag(cellFlags, WallFlags.South))
        {
            var wall = ecb.Instantiate(index, wallPrefab);
            ecb.SetComponent(index, wall, new LocalToWorldTransform
            {
                Value = UniformScaleTransform.FromPositionRotation(MazeUtils.GridPositionToWorld(x, y) + new float3(0, 0, -0.5f), quaternion.AxisAngle(math.up(), math.radians(180)))
            });
        }
        if (y == stride - 1 && MazeUtils.HasFlag(cellFlags, WallFlags.North))
        {
            var wall = ecb.Instantiate(index, wallPrefab);
            ecb.SetComponent(index, wall, new LocalToWorldTransform
            {
                Value = UniformScaleTransform.FromPositionRotation(MazeUtils.GridPositionToWorld(x, y) + new float3(0, 0, 0.5f), quaternion.identity)
            });
        }
    }
}

[BurstCompile]
public struct MazeGenJob : IJob
{
    public NativeArray<bool> VisitedCells;
    public DynamicBuffer<GridCell> Grid;

    [ReadOnly] public int MazeSize;
    [ReadOnly] public Random Random;

    [ReadOnly] public int2 InitialCell;
    
    public void Execute()
    { 
        var stack = new NativeList<int2>(Allocator.Temp);
        stack.Add(InitialCell);
        
        VisitedCells[MazeUtils.CellIdxFromPos(InitialCell, MazeSize)] = true;
        
        while (stack.Length > 0)
        {
            // pop
            var current = stack[stack.Length - 1];
            stack.RemoveAt(stack.Length - 1);
            
            // north, west, south, east
            var unvisitedNeighbors = new NativeList<int2>(Allocator.Temp);

            // west neighbor
            if (current.x > 0 && !VisitedCells[MazeUtils.CellIdxFromPos(current.x - 1, current.y, MazeSize)])
            {
                unvisitedNeighbors.Add(new int2(current.x - 1, current.y));
            }
            // south
            if (current.y > 0 && !VisitedCells[MazeUtils.CellIdxFromPos(current.x, current.y - 1, MazeSize)])
            {
                unvisitedNeighbors.Add(new int2(current.x, current.y - 1));
            }
            // east
            if (current.x < MazeSize - 1 && !VisitedCells[MazeUtils.CellIdxFromPos(current.x + 1, current.y, MazeSize)])
            {
                unvisitedNeighbors.Add(new int2(current.x + 1, current.y));
            }
            // north
            if (current.y < MazeSize - 1 && !VisitedCells[MazeUtils.CellIdxFromPos(current.x, current.y + 1, MazeSize)])
            {
                unvisitedNeighbors.Add(new int2(current.x, current.y + 1));
            }

            if (unvisitedNeighbors.Length > 0)
            {
                stack.Add(current);
                int2 next = unvisitedNeighbors[Random.NextInt(0, unvisitedNeighbors.Length)];
                // adds to end of list

                // remove wall between cells
                var currentCell = Grid[MazeUtils.CellIdxFromPos(current, MazeSize)];
                var nextCell = Grid[MazeUtils.CellIdxFromPos(next, MazeSize)];
                if (next.x > current.x)
                {
                    // next is to the east of current
                    currentCell.wallFlags &= (byte)~WallFlags.East;
                    nextCell.wallFlags &= (byte)~WallFlags.West;
                }
                else if (next.y > current.y)
                {
                    // next is to the north of current 
                    currentCell.wallFlags &= (byte)~WallFlags.North;
                    nextCell.wallFlags &= (byte)~WallFlags.South;
                }
                else if (next.x < current.x)
                {
                    // next is to the west of current
                    currentCell.wallFlags &= (byte)~WallFlags.West;
                    nextCell.wallFlags &= (byte)~WallFlags.East;
                }
                else
                {
                    // next is to the south of current
                    currentCell.wallFlags &= (byte)~WallFlags.South;
                    nextCell.wallFlags &= (byte)~WallFlags.North;
                }

                // update grid
                Grid[current.x + current.y * MazeSize] = currentCell;
                Grid[next.x + next.y * MazeSize] = nextCell;

                // mark chosen as visited
                VisitedCells[next.x + next.y * MazeSize] = true;

                stack.Add(next);
            }
        }
    }
}

[BurstCompile]
public struct ParallelMazeGenJob : IJobParallelFor
{
    [NativeDisableParallelForRestriction]
    public NativeArray<GridCell> Grid;

    [ReadOnly] public int MazeSize;
    [ReadOnly] public Random Random;
    
    public void Execute(int index)
    {
        var cell = Grid[index];
        int2 gridPos = MazeUtils.CellFromIndex(index, MazeSize);

        NativeList<byte> dirs = new NativeList<byte>(2, Allocator.Temp);
        if (gridPos.y < MazeSize) dirs.Add((byte)WallFlags.North);
        if (gridPos.x < MazeSize) dirs.Add((byte)WallFlags.East);

        var toRemove = dirs[Random.NextInt(0, dirs.Length)];
        cell.wallFlags &= (byte)~toRemove;

        if (gridPos.y > 0)
        {
            var cellBelow = Grid[MazeUtils.CellIdxFromPos(new int2(gridPos.x, gridPos.y - 1), MazeSize)];
            if (!MazeUtils.HasFlag((WallFlags) cellBelow.wallFlags, WallFlags.North))
                cell.wallFlags &= (byte) ~WallFlags.South;
        }
        if (gridPos.x > 0)
        {
            var cellLeft = Grid[MazeUtils.CellIdxFromPos(new int2(gridPos.x - 1, gridPos.y), MazeSize)];
            if (!MazeUtils.HasFlag((WallFlags) cellLeft.wallFlags, WallFlags.East))
                cell.wallFlags &= (byte) ~WallFlags.West;
        }

        Grid[index] = cell;
    }
}



[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
public partial struct MazeGeneratorSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gameConfig = SystemAPI.GetSingleton<GameConfig>();
        var random = new Random(gameConfig.seed);
        var gameConfigEntity = SystemAPI.GetSingletonEntity<GameConfig>();

        // create grid dynamic buffer
        var grid = state.EntityManager.AddBuffer<GridCell>(gameConfigEntity);
        grid.Resize(gameConfig.mazeSize * gameConfig.mazeSize, NativeArrayOptions.ClearMemory);

        // spawn floor
        var floorEntity = state.EntityManager.Instantiate(gameConfig.tile);
        state.EntityManager.SetComponentData(floorEntity, new LocalToWorldTransform
        {
            Value = UniformScaleTransform.FromPositionRotationScale(new float3(gameConfig.mazeSize / 2.0f, (-gameConfig.mazeSize / 2.0f) * 0.1f - 0.5f, gameConfig.mazeSize / 2.0f), quaternion.identity, gameConfig.mazeSize)
        });

        var size = gameConfig.mazeSize;
        var numCells = size * size;

        GridCell cell = new GridCell { wallFlags = (byte)WallFlags.None };
        cell.wallFlags = (byte)WallFlags.All;
        for(int i = 0; i < numCells; i++)
        {
            grid[i] = cell;
        }


        // REFERENCE: https://en.wikipedia.org/wiki/Maze_generation_algorithm#:~:text=to%20the%20stack-,Randomized%20Kruskal%27s%20algorithm,-%5Bedit%5D
        // 1. Choose initial cell, mark it as visited and push it to the stack
        // 2. While the stack is not empty
        //    1. Push the current cell to the stack
        //    2. Choose on the the unvisited neighbors
        //    3. Remove wall between current cell and chosen (next) cell
        //    4. Mark chosen (next) as visited and push it to the stack.
        
        
        JobHandle mazeGenHandle = default(JobHandle);

        var cellVisited = new NativeArray<bool>(numCells, Allocator.TempJob);
        if (!gameConfig.parallelMazeGen)
        {
            // NOTE: currently does not work with seeds > 1
            var initialCell = new int2(random.NextInt(0, size + 1), random.NextInt(0, size + 1));

            var job = new MazeGenJob
            {
                VisitedCells = cellVisited,
                Grid = grid,
                Random = random,
                InitialCell = initialCell,
                MazeSize = size
            };
            mazeGenHandle = job.Schedule();
            //cellVisited.Dispose();
        }
        else
        {
            var job = new ParallelMazeGenJob
            {
                Grid = grid.AsNativeArray(),
                Random = random,
                MazeSize = size
            };
            mazeGenHandle = job.Schedule(grid.Length, grid.Length / 32);
        }

        var mazeAreaLength = gameConfig.mazeSize - (2 * gameConfig.mazeStripWidth);
        var strideToPathCenter = mazeAreaLength / (gameConfig.openStripCount + 1);
        var halfPathWidth = gameConfig.openStripWidth / 2;
        
        var pathCenters = new NativeArray<int>(gameConfig.openStripCount, Allocator.Temp);
        for (int i = 0; i < gameConfig.openStripCount; i++)
        {
            // stride to the center
            pathCenters[i] = (i + 1) * strideToPathCenter;
        }

        var createMazeFreeZonesJob = new CreateMazeFreeZonesJob
        {
            grid = grid,
            stride = gameConfig.mazeSize,
            stripWidth = gameConfig.mazeStripWidth,
            pathCenters = new NativeArray<int>(pathCenters, Allocator.TempJob),
            halfPathWidth = halfPathWidth,
        };

        var createMazeFreeZonesHandle = createMazeFreeZonesJob.Schedule(numCells, 64, mazeGenHandle);        

        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        var spawnJob = new SpawnWalls
        {
            grid = grid,
            ecb = ecb,
            stride = size,
            wallPrefab = gameConfig.wallPrefab
        };

        var spawnHandle = spawnJob.Schedule(numCells, 64, createMazeFreeZonesHandle);
        spawnHandle.Complete();
        cellVisited.Dispose();

        //the amount of indices occupied by the moving walls.
        var movingWallLocationStack = new NativeList<int2>(Allocator.Temp);

        for (int i = 0; i < gameConfig.numMovingWalls; i++)
        {
            //select random start index
            var wallStartIndex = new int2(random.NextInt(0, size + 1), random.NextInt(0, size));
            //select for moving wall range
            var movingWallRange = random.NextInt(gameConfig.movingWallRangeMin, gameConfig.movingWallRangeMax);

            while (wallStartIndex.x + gameConfig.movingWallsLength + movingWallRange >= gameConfig.mazeSize)
            {
                wallStartIndex = new int2(random.NextInt(0, size + 1), random.NextInt(0, size + 1));
                movingWallRange = random.NextInt(gameConfig.movingWallRangeMin, gameConfig.movingWallRangeMax);
            }

            for (int j = 0; j < movingWallRange; j++)
            {
                //Clear out overlapping WallFlags
                MazeUtils.RemoveNorthSouthWall(wallStartIndex.x + j, wallStartIndex.y, ref grid, size);

                //Spawn Wall for length
                if (j < gameConfig.movingWallsLength)
                {
                    movingWallLocationStack.Add(new int2(wallStartIndex.x + j, wallStartIndex.y));

                    //Create Segment for wall
                    var movingWallSegment = state.EntityManager.Instantiate(gameConfig.movingWallPrefab);
                    state.EntityManager.SetComponentData(movingWallSegment, new LocalToWorldTransform
                    {
                        Value = UniformScaleTransform.FromPositionRotation(MazeUtils.GridPositionToWorld(wallStartIndex.x + j, wallStartIndex.y) - new float3(0, 0, 0.5f), quaternion.identity)
                    });
                    state.EntityManager.SetComponentData(movingWallSegment, new MovingWall
                    {
                        movementRange = movingWallRange
                    });
                    state.EntityManager.SetComponentData(movingWallSegment, new GridPositions()
                    {
                        gridStartX = wallStartIndex.x + j,
                        gridStartY = wallStartIndex.y,
                        gridEndX = wallStartIndex.x + j + movingWallRange,
                        gridEndY = wallStartIndex.y
                    });
                }
            }
        }

        //re add moving wall active segments
        while (movingWallLocationStack.IsEmpty != true)
        {
            var currentIndex = movingWallLocationStack[movingWallLocationStack.Length - 1];
            movingWallLocationStack.RemoveAt(movingWallLocationStack.Length - 1);

            MazeUtils.AddNorthSouthWall(currentIndex.x, currentIndex.y, ref grid, size);
        }

        // spawn player spawn point
        var playerSpawnPos = MazeUtils.GridPositionToWorld(
            random.NextInt(0, gameConfig.mazeSize - 1),
            random.NextInt(0, gameConfig.mazeSize - 1)
            );

        var playerSpawnEntity = state.EntityManager.Instantiate(gameConfig.playerSpawnPrefab);
        state.EntityManager.SetComponentData(playerSpawnEntity, new LocalToWorldTransform
        {
            Value = UniformScaleTransform.FromPositionRotation(playerSpawnPos + new float3(0, -0.4f, 0), quaternion.AxisAngle(math.right(), math.radians(90)))
        });


        state.Enabled = false;
    }
}