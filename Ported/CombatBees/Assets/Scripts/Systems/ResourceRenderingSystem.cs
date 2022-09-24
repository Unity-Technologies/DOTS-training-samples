using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[WithAll(typeof(GridPosition))]
partial struct ResourceScalingJob : IJobEntity
{
    void Execute(in TransformAspect prs, ref PostTransformMatrix mat)
    {
        var s = prs.LocalToWorld.Scale;
        mat.Value = float4x4.TRS(float3.zero, quaternion.identity, new float3(s, s * .5f, s));
    }
}

[BurstCompile]
partial struct ResourceRenderingSystem : ISystem
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
        new ResourceScalingJob().ScheduleParallel();
    }
}