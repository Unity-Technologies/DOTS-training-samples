using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct SpeedSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;
        float colliderSize = 1f;

        foreach (var (speed,transform) in SystemAPI.Query<RefRW<Speed>, TransformAspect>())
        {
            var nextPos = transform.LocalPosition + speed.ValueRO.speed * dt * 5;

            bool collided = false;


            float3 collidedPos = transform.LocalPosition;
            foreach (var wallTransform in SystemAPI.Query<TransformAspect>().WithAll<Wall>())
            {
                var diff = nextPos - wallTransform.LocalPosition;
                if(math.abs(diff.x) < colliderSize && math.abs(diff.y) < colliderSize && math.abs(diff.z) < colliderSize)
                {
                    collided = true;
                    collidedPos = wallTransform.LocalPosition;
                    break;
                }
            }

            if (collided)
            {
                var diffpos = transform.LocalPosition - collidedPos;
                speed.ValueRW.speed = math.reflect(math.normalize(speed.ValueRW.speed), math.normalize(diffpos)) * speed.ValueRW.bounceFactor * math.length(speed.ValueRW.speed);
            }

            transform.LocalPosition += speed.ValueRO.speed * dt * 5;
            speed.ValueRW.speed *= (1.0f-speed.ValueRW.dragFactor);
        }
    }
}
