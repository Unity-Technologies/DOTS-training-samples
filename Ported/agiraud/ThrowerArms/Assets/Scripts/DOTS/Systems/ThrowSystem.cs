using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using Random = Unity.Mathematics.Random;


[UpdateInGroup(typeof(ThrowerArmsGroupSystem))]
[UpdateBefore(typeof(PhysicsSystem))]
public class ThrowSystem : JobComponentSystem
{
    EntityQuery m_group;

    [BurstCompile]
    struct FindTargetAndThrowSystemJob : IJobForEach<Physics, Translation, ForceThrow>
    {
        [ReadOnly] public float gravityStrenth;
        [ReadOnly] public float3 CanVelocity;

        public void Execute(ref Physics physics, [ReadOnly] ref Translation translation, ref ForceThrow forcethrow)
        {
            if (physics.flying) return;
            if (math.abs(forcethrow.target.x) < 0.01f && math.abs(forcethrow.target.y) < 0.01f && math.abs(forcethrow.target.z) < 0.01f) return;

            physics.velocity = AimAtCan(ref forcethrow.target, CanVelocity, ref translation.Value, 20f);
            physics.flying = true;
            forcethrow.target = float3.zero;
        }

        float3 AimAtCan(ref float3 canPosition, float3 canVelocity, ref float3 startPos, float baseThrowSpeed)
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
        var job = new FindTargetAndThrowSystemJob();
        job.gravityStrenth = RockManagerAuthoring.RockGravityStrength;
        job.CanVelocity = SceneParameters.Instance.TinCanInitialVelocity;

        var jobHandle = job.Schedule(m_group, inputDeps);
        return jobHandle;
    }

    protected override void OnCreate()
    {
        m_group = GetEntityQuery(ComponentType.ReadWrite<Physics>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<RockTag>(), ComponentType.ReadWrite<ForceThrow>());
    }
}

