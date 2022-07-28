using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct SpawnFlashSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var dt = Time.deltaTime;
        foreach (var (spawnFlash, translation, scale)
            in SystemAPI.Query<Entity, RefRW<Translation>, RefRW<NonUniformScale>>().WithAll<SpawnFlash>())
        {
            scale.ValueRW.Value += new float3(0.03f, 0.03f, 0.03f) * dt;
            scale.ValueRW.Value *= new float3(1.1f, 1.1f, 1.1f);
            if (scale.ValueRO.Value.x >= 1.5f)
            {
                ecb.DestroyEntity(spawnFlash);
            }
            translation.ValueRW.Value += new float3(0, 2.5f, 0) * dt;

        }
    }
}
