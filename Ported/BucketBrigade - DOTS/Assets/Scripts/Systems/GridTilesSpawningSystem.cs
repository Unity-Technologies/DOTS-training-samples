using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct GridTilesSpawningSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
      
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var config = SystemAPI.GetSingleton<Config>();

        for (int i = 0; i <= config.columns; i++)
        {
            for (int j = 0; j <= config.rows; j++)
            {
                var groundTile = ecb.Instantiate(config.Ground);
                ecb.SetComponent(groundTile, new LocalTransform
                {
                    Position = new float3
                    {
                        x = i,
                        y = -3f,
                        z = j
                    },
                    Scale = 1f,
                    Rotation = quaternion.identity
                });
            }
        }
    }
}
