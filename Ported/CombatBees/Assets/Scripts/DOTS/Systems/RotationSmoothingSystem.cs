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
        Entities.ForEach((ref Rotation rotation, in Smoothing smoothing) =>
        {
            if (math.lengthsq(smoothing.SmoothDirection) > float.Epsilon)
            {
                rotation.Value = Quaternion.LookRotation(smoothing.SmoothDirection);
            }
        }).ScheduleParallel();
    }
}
