using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct SpawnFlashSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var dt = Time.deltaTime;
        foreach (var (spawnFlash, translation, scale)
            in SystemAPI.Query<Entity, RefRW<Translation>, RefRW<NonUniformScale>>().WithAll<SpawnFlash>())
        {
            scale.ValueRW.Value += new float3(3.0f, 3.0f, 3.0f) * dt;
            if (scale.ValueRO.Value.x >= 1.5f)
            {
                ecb.DestroyEntity(spawnFlash);
            }
            translation.ValueRW.Value += new float3(0, 4.0f, 0) * dt;

        }
    }
}
