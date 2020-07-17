using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public class RotationSmoothingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Rotation rotation, in Velocity velocity) =>
        {
            if (math.lengthsq(velocity.Value) > 0.01f)
            {
                rotation.Value = Quaternion.LookRotation(velocity.Value);
            }
        }).ScheduleParallel();
    }
}
