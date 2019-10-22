using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(ThrowerArmsGroupSystem))]
[UpdateBefore(typeof(GravitySystem))]
public class ThrowSystem : JobComponentSystem
{
    EntityQuery m_group;
    EntityCommandBufferSystem m_EntityCommandBufferSystem;

    //[BurstCompile]
    struct ThrowSystemJob : IJobForEachWithEntity<Mover, Translation>
    {
        public EntityCommandBuffer.Concurrent cmd;
        public float deltaTime;
        public Random rd;
        public float probability;

        public void Execute(Entity entity, int index, ref Mover mover, [ReadOnly] ref Translation translation)
        {
            if (rd.NextFloat() < probability)
            {
                cmd.AddComponent<FlyingTag>(index, entity);

                // impulse
                mover.velocity.y += rd.NextFloat(5, 30);
                mover.velocity.z += rd.NextFloat(1, 10);
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new ThrowSystemJob();
        job.cmd = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        job.deltaTime = Time.deltaTime;
        job.rd = new Random((uint)Environment.TickCount);
        job.probability = 0.1f;

        var jobHandle = job.Schedule(m_group, inputDeps);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_group = GetEntityQuery(ComponentType.ReadWrite<Mover>(), ComponentType.ReadOnly<Translation>(), ComponentType.Exclude<FlyingTag>(), ComponentType.ReadOnly<RockTag>());
    }
}
