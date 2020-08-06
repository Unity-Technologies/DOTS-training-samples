using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(CarInitializeSystem))]
public class CarRenderingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var segmentCollection = GetSingleton<SegmentCollection>();

        Entities
            .WithName("CarRenderingSystem")
            .ForEach((
                ref Translation translation,
                ref Rotation rotation,
                ref NonUniformScale scale,
                in Size size,
                in Offset offset,
                in CurrentSegment currentSegment,
                in Progress progress
            ) =>
            {
                //TRS
                translation.Value = CollisionSystem.CalculateCarPosition(segmentCollection,currentSegment,progress,offset);
                rotation.Value = quaternion.Euler(CollisionSystem.CalculateCarDirection(segmentCollection,currentSegment,progress,offset));
                scale.Value = size.Value;

            }).ScheduleParallel();
    }
}