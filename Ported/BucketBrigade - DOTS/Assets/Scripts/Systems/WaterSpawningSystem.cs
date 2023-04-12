using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct WaterSpawningSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var config = SystemAPI.GetSingleton<Config>();
        var random = SystemAPI.GetSingleton<Random>();

        for (int i = 0; i <= config.waterSourcesCount; i++)
        {
            var waterTile = ecb.Instantiate(config.Water);
            var capacity = random.Value.NextFloat(config.cellSize/2, 2*config.cellSize);

            //setting water source position
            if (random.Value.NextBool())//vert or horizontal
            {
                if (random.Value.NextBool())//top or bottom
                {
                    ecb.SetComponent(waterTile, new LocalTransform
                    {
                        Position = new float3
                        {
                            x = config.columns * config.cellSize + config.waterCellSize,
                            y = 0,
                            z = random.Value.NextFloat(-5,5 + config.rows * config.cellSize)
                        },
                        Scale = config.cellSize,
                        Rotation = quaternion.identity
                    });
                }
                else
                {
                    ecb.SetComponent(waterTile, new LocalTransform
                    {
                        Position = new float3
                        {
                            x = -config.columns * config.cellSize - config.waterCellSize,
                            y = 0,
                            z = random.Value.NextFloat(-5,5 + config.rows * config.cellSize)
                        },
                        Scale = config.cellSize,
                        Rotation = quaternion.identity
                    });
                } 
            }
            else
            {
                if (random.Value.NextBool())//right or left
                {
                    ecb.SetComponent(waterTile, new LocalTransform
                    {
                        Position = new float3
                        {
                            x = random.Value.NextFloat(-5,5 + config.columns * config.cellSize),
                            y = 0,
                            z = config.rows * config.cellSize + config.waterCellSize
                        },
                        Scale = config.cellSize,
                        Rotation = quaternion.identity
                    });
                }
                else
                {
                    ecb.SetComponent(waterTile, new LocalTransform
                    {
                        Position = new float3
                        {
                            x = random.Value.NextFloat(-5,5 + config.columns * config.cellSize),
                            y = 0,
                            z = -config.rows * config.cellSize - config.waterCellSize
                        },
                        Scale = capacity,
                        Rotation = quaternion.identity
                    });
                }
            }
        }
    }
}
