using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[WithAny(typeof(YellowTeam), typeof(BlueTeam))]
[WithNone(typeof(Decay))]
partial struct BeeScalingAndRotationJob : IJobEntity
{
    public float speedStretch;
    public float3 up;
    
    [BurstCompile]
    public void Execute(in Velocity velocity,
        in SmoothDirection smoothDirection,
        ref TransformAspect psr, ref PostTransformMatrix ptm)
    {
        float3 scale = new float3(psr.LocalToWorld.Scale,psr.LocalToWorld.Scale,psr.LocalToWorld.Scale);
        float magnitude = math.sqrt(velocity.Value.x * velocity.Value.x +
                                     velocity.Value.y * velocity.Value.y +
                                     velocity.Value.z * velocity.Value.z);
        float stretch = math.max(1f,magnitude * speedStretch);
        scale.z *= stretch;
        scale.x /= (stretch-1f)/5f+1f;
        scale.y /= (stretch-1f)/5f+1f;

        ptm.Value = float4x4.TRS(float3.zero, quaternion.LookRotation(smoothDirection.Value, up), scale);
    }
}