using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class ArmThrowSystem : JobComponentSystem
{
    [BurstCompile]
    static float GetCurveValue(float samplePos)
    {
        return math.clamp(sin(samplePos * 2.0f * 3.14f), 0f, 1f);
    }

    [BurstCompile]
    static float3 AimAtCan(float3 targetPosition, float3 velocity, float3 startPos)
    {
        float baseThrowSpeed = 24;
        // predictive aiming based on this article by Kain Shin:
        // https://www.gamasutra.com/blogs/KainShin/20090515/83954/Predictive_Aim_Mathematics_for_AI_Targeting.php

        float targetSpeed = math.length(velocity);
        float cosTheta = dot(normalize(startPos - targetPosition), normalize(velocity));
        float D = math.length(targetPosition - startPos);

        // quadratic equation terms
        float A = baseThrowSpeed * baseThrowSpeed - targetSpeed * targetSpeed;
        float B = (2f * D * targetSpeed * cosTheta);
        float C = -D * D;

        if (B * B < 4f * A * C)
        {
            // it's impossible to hit the target
            return float3(10, 8, 0);
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
            return float3(10, 8, 0);
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

        var output = velocity - .5f * new float3(0f, -25.0f, 0f) * t + (targetPosition - startPos) / t;

        if (length(output) > baseThrowSpeed * 2f)
        {
            // the required throw is too serious for us to handle
            return float3(10,8,0);
        }

        return output;
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

        var accessor = GetComponentDataFromEntity<Translation>(true);
        var velocityAccessor = GetComponentDataFromEntity<Velocity>(true);
        EntityCommandBuffer.Concurrent concurrentBuffer = entityCommandBuffer.ToConcurrent();
        var dt = Time.DeltaTime;
        var deps = Entities
            .WithReadOnly(accessor)
            .WithReadOnly(velocityAccessor)
            .ForEach((Entity entity, int entityInQueryIndex, ref ArmComponent arm, ref ThrowAtState throwAt, in Translation pos) =>
            {
                throwAt.ThrowTimer += dt / ArmConstants.ThrowDuration;
                
                // update our aim until we release the rock
                if (throwAt.HeldEntity != Entity.Null)
                {
                    throwAt.AimVector = AimAtCan(accessor[throwAt.AimedTargetEntity].Value, velocityAccessor[throwAt.AimedTargetEntity].Value, accessor[throwAt.HeldEntity].Value);
                }

                // we start this animation in our windup position,
                // and end it by returning to our default idle pose
                float3 restingPos = math.lerp(throwAt.StartPosition, arm.HandTarget, throwAt.ThrowTimer);

                // find the hand's target position to perform the throw
                // (somewhere forward and upward from the windup position)
                var throwHandTarget = throwAt.StartPosition + normalize(throwAt.AimVector) * 2.5f;

                arm.HandTarget = math.lerp(restingPos, throwHandTarget, GetCurveValue(throwAt.ThrowTimer));

                if (throwAt.ThrowTimer > .15f && throwAt.HeldEntity != Entity.Null)
                {
                    concurrentBuffer.RemoveComponent<GrabbedState>(entityInQueryIndex, throwAt.HeldEntity);
                    // release the rock
                    concurrentBuffer.AddComponent<Velocity>(entityInQueryIndex, throwAt.HeldEntity, new Velocity() { Value = throwAt.AimVector });
                    concurrentBuffer.AddComponent<FlyingState>(entityInQueryIndex, throwAt.HeldEntity);
                    concurrentBuffer.AddComponent<MarkedForDeath>(entityInQueryIndex, throwAt.HeldEntity, new MarkedForDeath { Timer = 10.0f }) ;
                    concurrentBuffer.AddComponent<Gravity>(entityInQueryIndex, throwAt.HeldEntity);

                    throwAt.HeldEntity = Entity.Null;
                }

                if (throwAt.ThrowTimer > 1f)
                {
                    concurrentBuffer.RemoveComponent<ThrowAtState>(entityInQueryIndex, entity);
                    concurrentBuffer.AddComponent<IdleState>(entityInQueryIndex, entity);
                    concurrentBuffer.AddComponent<FindGrabbableTargetState>(entityInQueryIndex, entity);
                }
            })
            .Schedule(inputDeps);

        return deps;
    }
}