using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using System.Collections;
using static Unity.Mathematics.math;
using Random = Unity.Mathematics.Random;


public class TornadoParticleSystem : JobComponentSystem
{
    private EntityQuery m_particleQuery;
    protected override void OnCreate()
    {
        m_particleQuery = GetEntityQuery( new EntityQueryDesc
        {
            All = new[] {ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<Rotation>(),ComponentType.ReadOnly<TornadoFlagComponent>()}
        });
    }

    [BurstCompile]
    [RequireComponentTag(typeof(TornadoData))]
    struct TornadoParticleSystemJob : IJobForEach<Translation, Rotation>
    {
        [ReadOnly] public float deltaTime;
        [ReadOnly] public float elapsedTime;
        [ReadOnly] public TornadoData tornado;
        [ReadOnly] public Random random;
        public void Execute(ref Translation translation, ref Rotation rotation)
        {
            float3 tornadoPos = new float3(
                Mathf.Cos(elapsedTime / 6f) * random.NextFloat(28, 30) + (Mathf.Sin((translation.Value.y) / 5f + elapsedTime / 4f) * random.NextFloat(2.5f, 3f)),
                translation.Value.y,
                Mathf.Sin((elapsedTime / 6f * 1.618f) * random.NextFloat(28, 30)));
            float3 delta = (tornadoPos - translation.Value);
            float dist = math.length(delta);
            delta /= dist;
            float inForce = dist - Mathf.Clamp01(tornadoPos.y / 50f) * 30f * tornado.radiusMultiplier + 2f;
            translation.Value += new float3(-delta.z * tornado.spinRate + delta.x * inForce, tornado.upwardSpeed, delta.x * tornado.spinRate + delta.z * inForce) * deltaTime;
            if (translation.Value.y > random.NextFloat(40, 50))
            {
                translation.Value = new Vector3(translation.Value.x, translation.Value.y - random.NextFloat(47, 53), translation.Value.z);
            }
            rotation.Value = Quaternion.AngleAxis(90, delta);
        }
    }

    // OnUpdate runs on the main thread.
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new TornadoParticleSystemJob
        {
            tornado = GetSingleton<TornadoData>(),
            deltaTime = Time.DeltaTime,
            elapsedTime = Time.time,
            random = new Random(5)
        };
        return job.Schedule(m_particleQuery, inputDependencies);
    }

}