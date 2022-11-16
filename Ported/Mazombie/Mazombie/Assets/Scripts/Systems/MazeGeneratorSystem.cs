using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

using UnityEngine;
using Random = Unity.Mathematics.Random;

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
        var random = new Random(42);

        var gameConfig = SystemAPI.GetSingleton<GameConfig>();
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
        bool[] cellVisited = new bool[numCells];

        GridCell cell = new GridCell { wallFlags = (byte)WallFlags.None };
        cell.wallFlags |= (byte)WallFlags.North;
        cell.wallFlags |= (byte)WallFlags.East;
        cell.wallFlags |= (byte)WallFlags.South;
        cell.wallFlags |= (byte)WallFlags.West;

        // all cells get all walls
        // TODO: jobify
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {

                grid[x + y * size] = cell;
                cellVisited[x + y * size] = false;
            }
        }

        var stack = new NativeList<int2>(Allocator.Temp);
        var current = new int2(random.NextInt(0, size + 1), random.NextInt(0, size + 1));
        cellVisited[current.x + current.y * size] = true;

        int numVisited = 1;

        // TODO: move to a job
        while (numVisited < numCells)
        {
            // north, west, south, east
            var unvisitedNeighbors = new NativeList<int2>(Allocator.Temp);

            // west neighbor
            if (current.x > 0 && !cellVisited[(current.x - 1) + current.y * size])
            {
                unvisitedNeighbors.Add(new int2(current.x - 1, current.y));
            }
            // north
            if (current.y > 0 && !cellVisited[(current.x) + (current.y - 1) * size])
            {
                unvisitedNeighbors.Add(new int2(current.x, current.y - 1));
            }
            // east
            if (current.x < size - 1 && !cellVisited[(current.x + 1) + current.y * size])
            {
                unvisitedNeighbors.Add(new int2(current.x + 1, current.y));
            }
            // south
            if (current.y < size - 1 && !cellVisited[current.x + (current.y + 1) * size])
            {
                unvisitedNeighbors.Add(new int2(current.x, current.y + 1));
            }

            if (unvisitedNeighbors.Length > 0)
            {
                int2 next = unvisitedNeighbors[random.NextInt(0, unvisitedNeighbors.Length)];
                // adds to end of list
                stack.Add(current);

                // remove wall between cells
                var currentCell = grid[current.x + current.y * size];
                var nextCell = grid[next.x + next.y * size];
                if (next.x > current.x)
                {
                    currentCell.wallFlags &= (byte)~WallFlags.East;
                    nextCell.wallFlags &= (byte)~WallFlags.West;
                }
                else if (next.y > current.y)
                {
                    currentCell.wallFlags &= (byte)~WallFlags.North;
                    nextCell.wallFlags &= (byte)~WallFlags.South;
                }
                else if (next.x < current.x)
                {
                    currentCell.wallFlags &= (byte)~WallFlags.West;
                    nextCell.wallFlags &= (byte)~WallFlags.East;
                }
                else
                {
                    currentCell.wallFlags &= (byte)~WallFlags.South;
                    nextCell.wallFlags &= (byte)~WallFlags.North;
                }

                cellVisited[next.x + next.y * size] = true;
                grid[current.x + current.y * size] = currentCell;
                grid[next.x + next.y * size] = nextCell;

                numVisited++;
                current = next;
            }
            else
            {
                if (stack.Length > 0)
                {
                    // get element at the end
                    current = stack[stack.Length - 1];
                    // remove
                    stack.RemoveAt(stack.Length - 1);
                }
            }
        }

        // remove strips of walls
        if (gameConfig.openStripWidth + gameConfig.mazeStripWidth > 0)
        {
            int offset = 0;
            for (; offset < size; offset += gameConfig.openStripWidth + gameConfig.mazeStripWidth)
            {
                // y goes along columns which is north to south
                for (int y = offset; y < math.min(offset + gameConfig.openStripWidth, size); y++)
                {
                    // x goes along rows from west to east
                    for (int x = 0; x < size; x++)
                    {
                        if (x > 0)
                        {
                            RemoveEastWestWall(x, y, ref grid, size);
                        }
                        if (y > offset)
                        {
                            RemoveNorthSouthWall(x, y, ref grid, size);
                        }
                    }
                }
            }
        }

        // remove walls around border
        //for (int y = 0; y < math.min(gameConfig.openStripWidth, size); y++)
        //{
        //    for (int x = 0; x < size; x++)
        //    {
        //        if (x > 0)
        //        {
        //            SetDownWall(x, y, ref grid, size);
        //        }
        //        if (y > 0)
        //        {
        //            SetLeftWall(x, y, ref grid, size);
        //        }
        //    }
        //}

        //for (int y = math.max(size - gameConfig.openStripWidth, 0); y < size; y++)
        //{
        //    for (int x = 0; x < size; x++)
        //    {
        //        if (x > 0)
        //        {
        //            SetDownWall(x, y, ref grid, size);
        //        }
        //        if (y > 0)
        //        {
        //            SetLeftWall(x, y, ref grid, size);
        //        }
        //    }
        //}

        //for (int x = 0; x < math.min(gameConfig.openStripWidth, size); x++)
        //{
        //    for (int y = 0; y < size; y++)
        //    {
        //        if (x > 0)
        //            SetDownWall(x, y, ref grid, size);
        //        if (y > 0)
        //            SetLeftWall(x, y, ref grid, size);
        //    }
        //}

        //for (int x = math.max(size - gameConfig.openStripWidth, 0); x < size; x++)
        //{
        //    for (int y = 0; y < size; y++)
        //    {
        //        if (x > 0)
        //            SetDownWall(x, y, ref grid, size);
        //        if (y > 0)
        //            SetLeftWall(x, y, ref grid, size);
        //    }
        //}

        // spawn walls
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                var tmp_cell = (WallFlags)grid[x + y * size].wallFlags;
                if (x == size - 1 && tmp_cell.HasFlag(WallFlags.North))
                {
                    var wall = state.EntityManager.Instantiate(gameConfig.wallPrefab);
                    state.EntityManager.SetComponentData(wall, new LocalToWorldTransform
                    {
                        Value = UniformScaleTransform.FromPositionRotation(MazeUtils.GridPositionToWorld(x, y) + new float3(0, 0, 0.5f), quaternion.identity)
                    });
                }
                if (tmp_cell.HasFlag(WallFlags.South))
                {
                    var wall = state.EntityManager.Instantiate(gameConfig.wallPrefab);
                    state.EntityManager.SetComponentData(wall, new LocalToWorldTransform
                    {
                        Value = UniformScaleTransform.FromPositionRotation(MazeUtils.GridPositionToWorld(x, y) - new float3(0, 0, 0.5f), quaternion.AxisAngle(math.up(), math.radians(180)))
                    });
                }
                if (tmp_cell.HasFlag(WallFlags.West))
                {
                    var wall = state.EntityManager.Instantiate(gameConfig.wallPrefab);
                    state.EntityManager.SetComponentData(wall, new LocalToWorldTransform
                    {
                        Value = UniformScaleTransform.FromPositionRotation(MazeUtils.GridPositionToWorld(x, y) + new float3(-0.5f, 0, 0), quaternion.AxisAngle(math.up(), math.radians(270)))
                    });

                }
                if (y == size - 1 && tmp_cell.HasFlag(WallFlags.East))
                {
                    var wall = state.EntityManager.Instantiate(gameConfig.wallPrefab);
                    state.EntityManager.SetComponentData(wall, new LocalToWorldTransform
                    {
                        Value = UniformScaleTransform.FromPositionRotation(MazeUtils.GridPositionToWorld(x, y) + new float3(0.5f, 0, 0), quaternion.AxisAngle(math.up(), math.radians(90)))
                    });
                }
            }
        }

        // spawn player spawn point
        var playerSpawnPos = MazeUtils.GridPositionToWorld(
            random.NextInt(0, gameConfig.mazeSize - 1),
            random.NextInt(0, gameConfig.mazeSize - 1)
            );

        var playerSpawnEntity = state.EntityManager.Instantiate(gameConfig.playerSpawnPrefab);
        state.EntityManager.SetComponentData(playerSpawnEntity, new LocalToWorldTransform
        {
            Value = UniformScaleTransform.FromPositionRotation(playerSpawnPos + new float3(0, -0.4f, 0), quaternion.AxisAngle(Vector3.right, Mathf.Deg2Rad * 90.0f))
        });


        state.Enabled = false;
    }

    void RemoveNorthSouthWall(int x, int y, ref DynamicBuffer<GridCell> grid, int size)
    {
        var r = x;
        var c = y - 1; // move north
        if (r < 0 || r >= size) return;
        if (c >= 0 && c < size)
        {
            var idx = MazeUtils.CellIdxFromPos(r, c, size);
            var tmp = grid[idx];
            tmp.wallFlags &= (byte)~WallFlags.North;
            grid[idx] = tmp;
        }
        if (c + 1 >= 0 && c + 1 < size)
        {
            var idx = MazeUtils.CellIdxFromPos(r, c + 1, size);
            var tmp = grid[idx];
            tmp.wallFlags &= (byte)~WallFlags.South;
            grid[idx] = tmp;
        }
    }

    void RemoveEastWestWall(int x, int y, ref DynamicBuffer<GridCell> grid, int size)
    {
        var r = x - 1;
        var c = y;
        if (c < 0 || c >= size) return;
        if (r >= 0 && r < size)
        {
            var idx = MazeUtils.CellIdxFromPos(r, c, size);
            var tmp = grid[idx];
            tmp.wallFlags &= (byte)~WallFlags.East;
            grid[idx] = tmp;
        }
        if (r + 1 >= 0 && r + 1 < size)
        {
            var idx = MazeUtils.CellIdxFromPos(r + 1, c, size);
            var tmp = grid[idx];
            tmp.wallFlags &= (byte)~WallFlags.West;
            grid[idx] = tmp;
        }
    }
}