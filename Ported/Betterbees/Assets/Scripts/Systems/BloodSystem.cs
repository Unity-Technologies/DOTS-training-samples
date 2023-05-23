using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct BloodSystem : ISystem
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
        // Store the destroy commands within a command buffer, this postpones structural changes while iterating through the query results
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        float bloodDecay = SystemAPI.Time.DeltaTime * Config.bloodDecay;
        foreach (var (transform, entity) in SystemAPI.Query<RefRW<LocalTransform>>()
            .WithAll<BloodComponent>()  // add blood component but don't access it
            .WithEntityAccess())    // get the entity id
        {
            transform.ValueRW.Scale -= bloodDecay;
            if (transform.ValueRW.Scale <= 0.0f)
            {
                ecb.DestroyEntity(entity);
            }
        }
    }
}
