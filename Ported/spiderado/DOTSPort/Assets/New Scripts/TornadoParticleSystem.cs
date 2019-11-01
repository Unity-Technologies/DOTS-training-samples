using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;


public class TornadoParticleSystem : JobComponentSystem
{
    private EntityQuery m_particleQuery;
    protected override void OnCreate()
    {
        m_particleQuery = GetEntityQuery( new EntityQueryDesc
        {
            All = new[] {ComponentType.ReadWrite<Translation>(), ComponentType.ReadOnly<TornadoFlagComponent>()}
        });
    }

    [BurstCompile]
    [RequireComponentTag(typeof(TornadoData))]
    struct TornadoParticleSystemJob : IJobForEach<Translation>
    {
        [ReadOnly] public float timeSinceStart;
        [ReadOnly] public TornadoData tornado;
        internal float deltaTime;

        public void Execute(ref Translation translation)
        {
            float3 tornadoPos = new float3(
                Mathf.Cos(timeSinceStart / 6f) * 30f + Mathf.Sin((translation.Value.y) / 5f + timeSinceStart / 4f) * 3f,
                translation.Value.y,
                Mathf.Sin(timeSinceStart / 6f * 1.618f) * 30f);
            float3 delta = (tornadoPos - translation.Value);
            float dist = math.length(delta);
            delta /= dist;
            float inForce = dist - Mathf.Clamp01(tornadoPos.y / 50f) * 30f * tornado.radiusMultiplier + 2f;
            translation.Value += new float3(-delta.z * tornado.spinRate + delta.x * inForce, tornado.upwardSpeed, delta.x * tornado.spinRate + delta.z * inForce) * timeSinceStart;
            if (translation.Value.y > 50f)
            {
                translation.Value = new Vector3(translation.Value.x, translation.Value.y - 50f, translation.Value.z);
            }
        }
    }

    // OnUpdate runs on the main thread.
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new TornadoParticleSystemJob
        {
            tornado = GetSingleton<TornadoData>(),
            timeSinceStart = Time.time,
            deltaTime = Time.DeltaTime
        };
        return job.Schedule(m_particleQuery, inputDependencies);
    }
}