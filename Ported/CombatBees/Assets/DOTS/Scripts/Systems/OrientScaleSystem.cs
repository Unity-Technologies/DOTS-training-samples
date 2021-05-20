using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(ApplyVelocitySystem))]
public class OrientScaleSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var upVector = new float3(0,1f,0);
        
        Entities
            .WithAll<IsOriented>()
            .ForEach((ref Rotation rotation, in Velocity velocity) =>
            {
                if (math.length(velocity.Value) == 0.0f)
                {
                    rotation.Value = quaternion.identity;
                }
                else
                {
                    // var normalizedVelocity = math.normalize(velocity.Value);
                    var beeRightVector = math.cross(upVector, velocity.Value);
                    var beeUpVector = math.cross(velocity.Value, beeRightVector);
                    rotation.Value = quaternion.LookRotation(velocity.Value, beeUpVector);
                }
            }).ScheduleParallel();

        Entities
            .WithAll<IsStretched>()
            .ForEach((ref NonUniformScale scale, in Velocity velocity) =>
            {
                //scale.Value.z = math.length(velocity.Value) / 7;       
            }).ScheduleParallel();
    }
}
