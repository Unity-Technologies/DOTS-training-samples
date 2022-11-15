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
        var random = Random.CreateFromIndex(123);
        
        var gameConfig = SystemAPI.GetSingleton<GameConfig>();
        var gameConfigEntity = SystemAPI.GetSingletonEntity<GameConfig>();
        
        // create grid dynamic buffer
        var grid = state.EntityManager.AddBuffer<GridCell>(gameConfigEntity);
        grid.Resize(gameConfig.mazeSize * gameConfig.mazeSize, NativeArrayOptions.ClearMemory);
        
        // spawn floor
        var floorEntity = state.EntityManager.Instantiate(gameConfig.tile);
        state.EntityManager.SetComponentData(floorEntity, new LocalToWorldTransform
        {
            Value = UniformScaleTransform.FromPositionRotationScale(new float3(gameConfig.mazeSize/2.0f, (-gameConfig.mazeSize/2.0f) * 0.1f - 0.5f, gameConfig.mazeSize/2.0f), quaternion.identity, gameConfig.mazeSize)
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

        var random = Random.CreateFromIndex(1234);

        var stack = new NativeList<int2>(Allocator.Temp);
        var current = new int2(random.NextInt(0, size + 1), random.NextInt(0, size + 1));
        cellVisited[current.x + current.y * size] = true;

        int numVisited = 1;
        
        // TODO: move to a job
        while(numVisited < numCells)
        {
            // north, west, south, east
            var unvisitedNeighbors = new NativeArray<int2>(4, Allocator.Temp);
            var count = 0;
            
            // west neighbor
            if (current.x > 0 && !cellVisited[(current.x - 1) + current.y * size])
            {
                unvisitedNeighbors[1] = new int2(current.x - 1, current.y);
                count++;
            }
            // north
            if (current.y > 0 && !cellVisited[(current.x) + (current.y - 1) * size])
            {
                unvisitedNeighbors[0] = new int2(current.x, current.y - 1);
                count++;
            }
            // east
            if (current.x < size - 1 && !cellVisited[(current.x + 1) + current.y * size])
            {
                unvisitedNeighbors[3] = new int2(current.x + 1, current.y);
                count++;
            }
            // south
            if (current.y < size - 1 && !cellVisited[current.x + (current.y + 1) * size])
            {
                unvisitedNeighbors[2] = new int2(current.x, current.y + 1);
                count++;
            }

            if(count > 0)
            {
                int2 next = unvisitedNeighbors[random.NextInt(0, count + 1)];
                // adds to end of list
                stack.Add(current);

                // remove wall between cells
                var currentCell = grid[current.x + current.y * size];
                var nextCell = grid[next.x + next.y * size];
                if(next.x > current.x)
                {
                    currentCell.wallFlags &= (byte)~WallFlags.East;
                    nextCell.wallFlags &= (byte)~WallFlags.West;
                }
                else if(next.y > current.y)
                {
                    currentCell.wallFlags &= (byte)~WallFlags.North;
                    nextCell.wallFlags &= (byte)~WallFlags.South;
                }
                else if(next.x < current.x)
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
                if(stack.Length > 0)
                {
                    // get element at the end
                    current = stack[^1];
                    // remove
                    stack.RemoveAt(stack.Length - 1);
                }
            }
        }

        // override the border walls
        //for (int y = 0; y < gameConfig.mazeSize; y++)
        //{
        //    for (int x = 0; x < gameConfig.mazeSize; x++)
        //    {
        //        GridCell tmp_cell = new GridCell { wallFlags = (byte)WallFlags.None };
        //        bool update_cell = false;

        //        if (x == 0)
        //        {
        //            tmp_cell.wallFlags |= (byte)WallFlags.West;
        //            update_cell = true;
        //        }
        //        if (y == 0)
        //        {
        //            tmp_cell.wallFlags |= (byte)WallFlags.North;
        //            update_cell = true;

        //        }
        //        if (x == gameConfig.mazeSize - 1)
        //        {
        //            tmp_cell.wallFlags |= (byte)WallFlags.East;
        //            update_cell = true;

        //        }
        //        if (y == gameConfig.mazeSize - 1)
        //        {
        //            tmp_cell.wallFlags |= (byte)WallFlags.South;
        //            update_cell = true;
        //        }

        //        if(update_cell)
        //            grid[x + y * gameConfig.mazeSize] = tmp_cell;
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
                        Value = UniformScaleTransform.FromPositionRotation(GridPositionToWorld(x, y) - new float3(0, 0, 0.5f), quaternion.identity)
                    });
                }
                if (tmp_cell.HasFlag(WallFlags.South))
                {
                    var wall = state.EntityManager.Instantiate(gameConfig.wallPrefab);
                    state.EntityManager.SetComponentData(wall, new LocalToWorldTransform
                    {
                        Value = UniformScaleTransform.FromPositionRotation(GridPositionToWorld(x, y) + new float3(0, 0, 0.5f), quaternion.AxisAngle(math.up(), math.radians(180)))
                    });
                }
                if (tmp_cell.HasFlag(WallFlags.West))
                {
                    var wall = state.EntityManager.Instantiate(gameConfig.wallPrefab);
                    state.EntityManager.SetComponentData(wall, new LocalToWorldTransform
                    {
                        Value = UniformScaleTransform.FromPositionRotation(GridPositionToWorld(x, y) + new float3(-0.5f, 0, 0), quaternion.AxisAngle(math.up(), math.radians(270)))
                    });

                }
                if (y == size - 1 && tmp_cell.HasFlag(WallFlags.East))
                {
                    var wall = state.EntityManager.Instantiate(gameConfig.wallPrefab);
                    state.EntityManager.SetComponentData(wall, new LocalToWorldTransform
                    {
                        Value = UniformScaleTransform.FromPositionRotation(GridPositionToWorld(x, y) + new float3(0.5f, 0, 0), quaternion.AxisAngle(math.up(), math.radians(90)))
                    });
                }
            }
        }
        
        // spawn player spawn point
        var playerSpawnPos = GridPositionToWorld(
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

    public int GetGridIndex(int x, int y, int size)
    {
        return x + (y * size);
    }

    public float3 GridPositionToWorld(int x, int y)
    {
        return new float3(x + 0.5f, 0, y + 0.5f);
    }
}