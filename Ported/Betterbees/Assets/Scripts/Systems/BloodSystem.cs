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
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Store the destroy commands within a command buffer, this postpones structural changes while iterating through the query results
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var config = SystemAPI.GetSingleton<Config>();

        BloodJob bloodJob = new BloodJob()
        {
            lowerBounds = -config.bounds.y,
            ecb = ecb.AsParallelWriter()
        };
        bloodJob.ScheduleParallel();
    }
}

[WithAll(typeof(BloodComponent)), WithNone(typeof(DecayComponent))]
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
            transform.Position.y = lowerBounds;
            ecb.SetComponentEnabled<DecayComponent>(chunkIndex, entity, true);
        }

        if (transform.Position.y < lowerBounds)
        {
            Debug.Log("Error");
        }
    }
}
