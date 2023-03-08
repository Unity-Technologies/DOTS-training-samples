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

        {
            var waterTile = ecb.Instantiate(config.Water);
            ecb.SetComponent(waterTile, new LocalTransform
            {
                Position = new float3
                {
                    x = 10 + config.rows,
                    y = 0,
                    z = 10 + config.columns
                },
                Scale = 10f,
                Rotation = quaternion.identity
            });
            
        }
    }
}
