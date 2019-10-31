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
        [ReadOnly] public float deltaTime;
        [ReadOnly] public TornadoData tornado;
        public void Execute(ref Translation translation)
        {
            float3 tornadoPos = new float3(
                Mathf.Cos(deltaTime / 6f) * 30f + Mathf.Sin((translation.Value.y) / 5f + deltaTime / 4f) * 3f,
                translation.Value.y,
                Mathf.Sin(deltaTime / 6f * 1.618f) * 30f);
            float3 delta = (tornadoPos - translation.Value);
            float dist = math.length(delta);
            delta /= dist;
            float inForce = dist - Mathf.Clamp01(tornadoPos.y / 50f) * 30f * tornado.radiusMultiplier + 2f;
            translation.Value += new float3(-delta.z * tornado.spinRate + delta.x * inForce, tornado.upwardSpeed, delta.x * tornado.spinRate + delta.z * inForce) * deltaTime;
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
            deltaTime = Time.deltaTime
        };
        return job.Schedule(m_particleQuery, inputDependencies);
    }

}