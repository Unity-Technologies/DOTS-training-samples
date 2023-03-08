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

        {
            var waterTile = ecb.Instantiate(config.Water);
            if (random.Value.NextBool())//vert or horizontal
            {
                if (random.Value.NextBool())//top or bottom
                {
                    ecb.SetComponent(waterTile, new LocalTransform
                    {
                        Position = new float3
                        {
                            x = 5 + config.columns,
                            y = 0,
                            z = random.Value.NextInt(-5,5 + config.rows)
                        },
                        Scale = 5f,
                        Rotation = quaternion.identity
                    });
                }
                else
                {
                    ecb.SetComponent(waterTile, new LocalTransform
                    {
                        Position = new float3
                        {
                            x = -5,
                            y = 0,
                            z = random.Value.NextInt(-5,5 + config.rows)
                        },
                        Scale = 5f,
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
                            x = random.Value.NextInt(-5,5 + config.columns),
                            y = 0,
                            z = 5 + config.rows
                        },
                        Scale = 5f,
                        Rotation = quaternion.identity
                    });
                }
                else
                {
                    ecb.SetComponent(waterTile, new LocalTransform
                    {
                        Position = new float3
                        {
                            x = random.Value.NextInt(-5,5 + config.columns),
                            y = 0,
                            z = -5
                        },
                        Scale = 5f,
                        Rotation = quaternion.identity
                    });
                }
            }
        }
    }
}
