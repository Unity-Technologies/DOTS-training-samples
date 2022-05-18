using System.Diagnostics;
using System.Resources;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct PlatformSpawnSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        foreach (var buffer in SystemAPI.Query<DynamicBuffer<Platform>>())
        {
            var array = buffer.AsNativeArray();

            for (int i = 0; i < array.Length; i++)
            {
                var rotation = Quaternion.LookRotation(math.normalize(array[i].endWorldPosition - array[i].startWorldPosition)) * Quaternion.Euler(0f,90f,0f);

                var platform = ecb.Instantiate(config.PlatformPrefab);
                ecb.SetComponent(platform, new Translation { Value = array[i].startWorldPosition });
                ecb.SetComponent(platform, new Rotation { Value = rotation });
            }
        }
        
        // This system should only run once at startup. So it disables itself after one update.
        state.Enabled = false;
    }
}