using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(GravitySystem))]
public class OrientScaleSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var upVector = new float3(0, 1, 0);
        
        Entities
            .WithName("RotateBee")
            .WithAll<IsOriented>()
            .ForEach((ref Rotation rotation, in Velocity velocity) =>
            {
                rotation.Value = quaternion.LookRotation(velocity.Value, upVector);
            }).Schedule();

        Entities
            .WithName("ScaleBee")
            .WithAll<IsStretched>()
            .ForEach((ref NonUniformScale scale, in Velocity velocity) =>
            {
                scale.Value.z = math.length(velocity.Value) / 7;
            }).Schedule();
    }
}
