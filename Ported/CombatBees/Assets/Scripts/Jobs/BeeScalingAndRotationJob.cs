using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[WithAny(typeof(YellowTeam), typeof(BlueTeam))]
partial struct BeeScalingAndRotationJob : IJobEntity
{
    public float speedStretch;
    [ReadOnly] public ComponentLookup<Decay> isDeadLookup;
    
    public void Execute(in Entity entity, in Velocity velocity, in SmoothDirection smoothDirection, in LocalToWorld l2W, ref TransformAspect psr, ref PostTransformMatrix ptm)
    {
        float3 scale = new float3(psr.LocalToWorld.Scale,psr.LocalToWorld.Scale,psr.LocalToWorld.Scale);
        float magnitude = Mathf.Sqrt(velocity.Value.x * velocity.Value.x +
                                     velocity.Value.y * velocity.Value.y +
                                     velocity.Value.z * velocity.Value.z);

        float stretch = Mathf.Max(1f,magnitude * speedStretch);

        if (!isDeadLookup.HasComponent(entity))
        {
            scale.z *= stretch;
            scale.x /= (stretch-1f)/5f+1f;
            scale.y /= (stretch-1f)/5f+1f;
        }
        
        ptm = new PostTransformMatrix
        {
            Value = float4x4.TRS(float3.zero, quaternion.LookRotation(smoothDirection.Value, l2W.Up), scale)
        };
    }
}