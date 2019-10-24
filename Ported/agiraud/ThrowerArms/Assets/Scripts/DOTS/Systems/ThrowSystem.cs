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
[UpdateBefore(typeof(PhysicsSystem))]
public class ThrowSystem : JobComponentSystem
{
    EntityQuery m_group;
    EntityQuery m_CansQuery;

    [BurstCompile]
    struct GenerateTargetCanArrayJob : IJobForEachWithEntity<Translation>
    {
        public NativeHashMap<Entity, float3>.ParallelWriter targets;

        public void Execute(Entity entity, int index, ref Translation position)
        {
            targets.TryAdd(entity, position.Value);
        }
    }

    [BurstCompile]
    struct FindTargetAndThrowSystemJob : IJobForEachWithEntity<Physics, Translation>
    {
        [ReadOnly] public float deltaTime;
        public Random rd;
        [ReadOnly] public float probability;
        [ReadOnly] public float gravityStrenth;
        [ReadOnly] public float3 CanVelocity;
        [ReadOnly]public NativeArray<float3> targets;

        public void Execute(Entity entity, int index, ref Physics physics, [ReadOnly] ref Translation translation)
        {
            if (physics.flying) return;
            if (rd.NextFloat(0, 100) < probability)
            {
                float3 nearestTarget = FindNearestTarget(ref translation.Value, ref targets);
                physics.velocity = AimAtCan(ref nearestTarget, CanVelocity, ref translation.Value, 20f);
                physics.flying = true;
            }
        }

        float3 FindNearestTarget(ref float3 translationValue, ref NativeArray<float3> targets)
        {
            float nearestDistance = math.lengthsq(translationValue - targets[0]);
            float3 nearestTarget = targets[0];
            for (int i = 1; i < targets.Length; i++)
            {
                float dist = math.lengthsq(translationValue - targets[i]);
                if (dist < nearestDistance)
                {
                    nearestDistance = dist;
                    nearestTarget = targets[i];
                }
            }
            return nearestTarget;
        }

        public float3 AimAtCan(ref float3 canPosition, float3 canVelocity, ref float3 startPos, float baseThrowSpeed)
        {

            // predictive aiming based on this article by Kain Shin:
            // https://www.gamasutra.com/blogs/KainShin/20090515/83954/Predictive_Aim_Mathematics_for_AI_Targeting.php

            float targetSpeed = math.length(canVelocity);
            float cosTheta = math.dot(math.normalize(startPos - canPosition), math.normalize(canVelocity));

            float D = math.length(canPosition - startPos);

            // quadratic equation terms
            float A = baseThrowSpeed * baseThrowSpeed - targetSpeed * targetSpeed;
            float B = (2f * D * targetSpeed * cosTheta);
            float C = -D * D;

            if (B * B < 4f * A * C)
            {
                // it's impossible to hit the target
                return math.forward(quaternion.identity) * 10f + math.up() * 8f;
            }

            // quadratic equation has two possible outputs
            float t1 = (-B + math.sqrt(B * B - 4f * A * C)) / (2f * A);
            float t2 = (-B - math.sqrt(B * B - 4f * A * C)) / (2f * A);

            // our two t values represent two possible trajectory durations.
            // pick the best one - whichever is lower, as long as it's positive
            float t;
            if (t1 < 0f && t2 < 0f)
            {
                // both potential collisions take place in the past!
                return math.forward(quaternion.identity) * 10f + math.up() * 8f;
            }
            else if (t1 < 0f && t2 > 0f)
            {
                t = t2;
            }
            else if (t1 > 0f && t2 < 0f)
            {
                t = t1;
            }
            else
            {
                t = math.min(t1, t2);
            }

            float3 output = canVelocity - .5f * new float3(0f, -gravityStrenth, 0f) * t + (canPosition - startPos) / t;

            if (math.length(output) > baseThrowSpeed * 2f)
            {
                // the required throw is too serious for us to handle
                return math.forward(quaternion.identity) * 10f + math.up() * 8f;
            }

            return output;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var rockCount = m_CansQuery.CalculateEntityCount();
        if (rockCount == 0)
            return inputDeps;

        var jobArray = new GenerateTargetCanArrayJob();
        var targetHashMap = new NativeHashMap<Entity, float3>(rockCount, Allocator.TempJob);
        jobArray.targets = targetHashMap.AsParallelWriter();
        jobArray.Schedule(m_CansQuery, inputDeps).Complete();

        var job = new FindTargetAndThrowSystemJob();
        job.deltaTime = Time.deltaTime;
        job.rd = new Random((uint)Environment.TickCount);
        job.probability = 1f;
        job.gravityStrenth = RockManagerAuthoring.RockGravityStrength;
        job.CanVelocity = SceneParameters.Instance.TinCanInitialVelocity;
        job.targets = targetHashMap.GetValueArray(Allocator.TempJob);

        var jobHandle = job.Schedule(m_group, inputDeps);
        jobHandle.Complete();

        targetHashMap.Dispose();
        job.targets.Dispose();

        return inputDeps;
    }

    protected override void OnCreate()
    {
        m_CansQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadOnly<TinCanTag>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadWrite<Physics>() },
        });
        m_group = GetEntityQuery(ComponentType.ReadWrite<Physics>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<RockTag>());
    }
}
