using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(TargetSelectionSystem))]
public class RockVelocitySystem : SystemBase
{
    protected override void OnUpdate()
    {
        var velocities = GetComponentDataFromEntity<Velocity>();
        var translations = GetComponentDataFromEntity<Translation>();

        EntityCommandBufferSystem sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer ecb = sys.CreateCommandBuffer();
        var ecbParaWriter = ecb.AsParallelWriter();

        Entities
            .WithAll<HandThrowingRock>()
            .WithReadOnly(velocities)
            .WithReadOnly(translations)
            .ForEach((Entity entity, int entityInQueryIndex, ref TargetCan targetCan, in TargetRock targetRock, in Timer timer, in TargetPosition targetPos, in Translation translation) =>
            {
                if (targetCan.Value != Entity.Null && timer.Value <= 0.15f && timer.Value >= 0.0f)
                {
            		const float baseThrowSpeed = 24.0f;
                    float3 rockVelocity = float3.zero;
                    float3 canVelocity = velocities[targetCan.Value].Value;
                    float3 canTranslation = translations[targetCan.Value].Value;
                    float canSpeed = Unity.Mathematics.math.length(canVelocity);
                    float cosTheta = Unity.Mathematics.math.dot(Unity.Mathematics.math.normalize(translation.Value - canTranslation), Unity.Mathematics.math.normalize(canVelocity));
                    float D = Unity.Mathematics.math.length(canTranslation - translation.Value);

                    // quadratic equation terms
                    float A = baseThrowSpeed * baseThrowSpeed - canSpeed * canSpeed;
                    float B = (2f * D * canSpeed * cosTheta);
                    float C = -D * D;

                    if (B * B < 4f * A * C)
                    {
                        rockVelocity.z = 10.0f;
                        rockVelocity.y = 8.0f;
                    }
                    else
                    {
                        float t1 = (-B + Unity.Mathematics.math.sqrt(B * B - 4f * A * C)) / (2f * A);
                        float t2 = (-B - Unity.Mathematics.math.sqrt(B * B - 4f * A * C)) / (2f * A);

                        float t = 0.0f;
                        if (t1 < 0f && t2 < 0f)
                        {
                            // both potential collisions take place in the past!
                            rockVelocity.z = 10.0f;
                            rockVelocity.y = 8.0f;
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
                            t = Unity.Mathematics.math.min(t1, t2);
                        }

                        rockVelocity = canVelocity - .5f * new float3(0f, -9.8f, 0f) * t + (canTranslation - translation.Value) / t;

                        //if (Unity.Mathematics.math.lengthsq(rockVelocity) > baseThrowSpeed * 2f)
                        //{
                        //    // the required throw is too serious for us to handle
                        //    rockVelocity.z = 10.0f;
                        //    rockVelocity.y = 8.0f;
                        //}
                        //velocities[targetRock.RockEntity] = new Velocity { Value = rockVelocity };
                    }
                    ecbParaWriter.SetComponent<Velocity>(entityInQueryIndex, targetRock.RockEntity, new Velocity { Value = rockVelocity });
                    ecbParaWriter.AddComponent<Falling>(entityInQueryIndex, targetRock.RockEntity);
                }
            }).ScheduleParallel();

        sys.AddJobHandleForProducer(Dependency);

    }
}
