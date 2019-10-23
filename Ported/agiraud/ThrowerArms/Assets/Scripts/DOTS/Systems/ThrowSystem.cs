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
//[UpdateBefore(typeof(GravitySystem))]
public class ThrowSystem : JobComponentSystem
{
    EntityQuery m_group;

    [BurstCompile]
    struct ThrowSystemJob : IJobForEachWithEntity<Physics, Translation>
    {
        public float deltaTime;
        public Random rd;
        public float probability;

        public void Execute(Entity entity, int index, ref Physics physics, [ReadOnly] ref Translation translation)
        {
            if (physics.flying) return;
            if (rd.NextFloat(0,100) < probability)
            {
                // impulse
                physics.velocity.y = rd.NextFloat(5, 30);
                physics.velocity.z = rd.NextFloat(1, 20);
                physics.flying = true;
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new ThrowSystemJob();
        job.deltaTime = Time.deltaTime;
        job.rd = new Random((uint)Environment.TickCount);
        job.probability = 1f;

        var jobHandle = job.Schedule(m_group, inputDeps);

        return jobHandle;
    }

    protected override void OnCreate()
    {
        m_group = GetEntityQuery(ComponentType.ReadWrite<Physics>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<RockTag>());
    }
}
