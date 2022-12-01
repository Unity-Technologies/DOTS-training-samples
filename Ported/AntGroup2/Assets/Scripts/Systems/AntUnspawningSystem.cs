using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

[UpdateAfter(typeof(AntMovementSystem))]
partial struct AntUnspawningSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }


    public void OnDestroy(ref SystemState state)
    {
    }


    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var job = new UnspawnJob
        {
            halfSize = config.PlaySize * 0.5f,
            Ecb = ecb.AsParallelWriter()
        };

        job.ScheduleParallel();
    }

    [BurstCompile]
    [WithAll(typeof(Ant))]
    partial struct UnspawnJob : IJobEntity
    {
        public float halfSize;
        public EntityCommandBuffer.ParallelWriter Ecb;

        public void Execute([ChunkIndexInQuery]int index, TransformAspect transform, Entity entity)
        {
            if (math.abs(transform.WorldPosition.x) > halfSize || math.abs(transform.WorldPosition.z) > halfSize)
            {
                Ecb.DestroyEntity(index, entity);
            }
        }
    }
}
