using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
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

        // generate the outer wall
        for(int y = 0; y < gameConfig.mazeSize; y++)
        {
            for(int x = 0; x < gameConfig.mazeSize; x++)
            {
                GridCell cell = new GridCell { wallFlags = (byte)WallFlags.None};

                if (x == 0)
                    cell.wallFlags |= (byte)WallFlags.West;
                if (y == 0)
                    cell.wallFlags |= (byte)WallFlags.North;
                if(x == gameConfig.mazeSize - 1)
                    cell.wallFlags |= (byte)WallFlags.East;
                if (y == gameConfig.mazeSize - 1)
                    cell.wallFlags |= (byte)WallFlags.South;
                if (x == gameConfig.mazeSize/2 && y > 2)
                    cell.wallFlags |= (byte)WallFlags.East;
                
                grid[x + y * gameConfig.mazeSize] = cell;
            }
        }

        // spawn outer wall
        for (int y = 0; y < gameConfig.mazeSize; y++)
        {
            for (int x = 0; x < gameConfig.mazeSize; x++)
            {
                var cell = (WallFlags)grid[x + y * gameConfig.mazeSize].wallFlags;
                if (cell.HasFlag(WallFlags.North))
                {
                    var outerWall = state.EntityManager.Instantiate(gameConfig.wallPrefab);
                    state.EntityManager.SetComponentData(outerWall, new LocalToWorldTransform
                    {
                        Value = UniformScaleTransform.FromPositionRotation(GridPositionToWorld(x, y) - new float3(0, 0, 0.5f), quaternion.identity)
                    });
                }
                if (cell.HasFlag(WallFlags.South))
                {
                    var outerWall = state.EntityManager.Instantiate(gameConfig.wallPrefab);
                    state.EntityManager.SetComponentData(outerWall, new LocalToWorldTransform
                    {
                        Value = UniformScaleTransform.FromPositionRotation(GridPositionToWorld(x, y) + new float3(0, 0, 0.5f), quaternion.AxisAngle(Vector3.up, Mathf.Deg2Rad * 180))
                    });
                }
                if(cell.HasFlag(WallFlags.West))
                {
                    var outerWall = state.EntityManager.Instantiate(gameConfig.wallPrefab);
                    state.EntityManager.SetComponentData(outerWall, new LocalToWorldTransform
                    {
                        Value = UniformScaleTransform.FromPositionRotation(GridPositionToWorld(x, y) + new float3(-0.5f, 0, 0), quaternion.AxisAngle(Vector3.up, Mathf.Deg2Rad * 270))
                    });

                }
                if(cell.HasFlag(WallFlags.East))
                {
                    var outerWall = state.EntityManager.Instantiate(gameConfig.wallPrefab);
                    state.EntityManager.SetComponentData(outerWall, new LocalToWorldTransform
                    {
                        Value = UniformScaleTransform.FromPositionRotation(GridPositionToWorld(x, y) + new float3(0.5f, 0, 0), quaternion.AxisAngle(Vector3.up, Mathf.Deg2Rad * 90))
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

    public float3 GridPositionToWorld(int x, int y)
    {
        return new float3(x + 0.5f, 0, y + 0.5f);
    }
}