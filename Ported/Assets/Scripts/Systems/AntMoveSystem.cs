using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Profiling;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(SteeringSystem))]
public partial class AntMoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        Entities.WithAll<AntTag>().ForEach((Entity entity, ref Translation translation, ref Rotation rotation, in Velocity velocity) =>
        {
            var deltaPosition = velocity.Direction * velocity.Speed * deltaTime;

            var newPosition = translation.Value.xy + deltaPosition;
            translation.Value = new float3(newPosition, 0);
            
            rotation.Value = quaternion.LookRotation(new float3(velocity.Direction, 0), new float3(0, 0, 1));
        }).ScheduleParallel();
    }
}
