using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(BeeSystem))]
public partial struct BloodSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Store the destroy commands within a command buffer, this postpones structural changes while iterating through the query results
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var config = SystemAPI.GetSingleton<Config>();

        float bloodDecay = SystemAPI.Time.DeltaTime * config.bloodDecay;
        /*
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
        */

        BloodJob bloodJob = new BloodJob()
        {
            bloodDecay = bloodDecay,
            lowerBounds = -config.bounds.y,
            ecb = ecb.AsParallelWriter()
        };
        bloodJob.ScheduleParallel();
    }
}

[WithAll(typeof(BloodComponent))]
[BurstCompile]
public partial struct BloodJob : IJobEntity
{
    public float bloodDecay;
    public float lowerBounds;
    public EntityCommandBuffer.ParallelWriter ecb;

    public void Execute(ref LocalTransform transform, Entity entity, [ChunkIndexInQuery]int chunkIndex)
    {
        if (transform.Position.y <= lowerBounds)
        {
            transform.Scale -= bloodDecay;
            transform.Position.y = lowerBounds;
            if (transform.Scale <= 0.0f)
            {
                ecb.DestroyEntity(chunkIndex, entity);
            }
        }

        if (transform.Position.y < lowerBounds)
        {
            Debug.Log("Error");
        }
    }
}
