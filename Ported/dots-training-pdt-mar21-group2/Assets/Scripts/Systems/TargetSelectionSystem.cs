using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

public class TargetSelectionSystem : SystemBase
{

    protected override void OnUpdate()
    {
        var velocities = GetComponentDataFromEntity<Velocity>();

        Entities.
            WithAll<HandThrowingRock>()
            .ForEach((in Timer timer, in TargetRock targetRock, in TargetCan targetCan, in TargetPosition targetPos, in Translation translation) =>
            {
                if (timer.Value <= 0.15f)
                {
                    float3 rockVelocity;
                    float3 canVelocity = velocities[targetCan.Value].Value;
                    float canSpeed = Unity.Mathematics.math.lengthsq(canVelocity);
                    float cosTheta = Unity.Mathematics.math.dot(Unity.Mathematics.math.normalize(translation.Value - targetPos.Value), Unity.Mathematics.math.normalize(canVelocity));
                    float D = Unity.Mathematics.math.lengthsq(targetPos.Value - translation.Value);

                    // quadratic equation terms
                    float A = 10 * 10 - canSpeed * canSpeed;
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

                        rockVelocity = canVelocity - .5f * new float3(0f, -9.8f, 0f) * t + (targetPos.Value - translation.Value) / t;

                        if (Unity.Mathematics.math.lengthsq(rockVelocity) > 10 * 2f)
                        {
                            // the required throw is too serious for us to handle
                            rockVelocity.z = 10.0f;
                            rockVelocity.y = 8.0f;
                        }

                        velocities[targetRock.RockEntity] = new Velocity { Value = rockVelocity };
                    }
                }
            }).ScheduleParallel();
    }
}
