using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(CarMovementSystem))]
public class CarRenderingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var segmentCollection = GetSingleton<SegmentCollection>();

        Entities
            .WithName("CarRenderingSystem")
            .WithNone<Finished>()
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
                var forward = CollisionSystem.CalculateCarDirection(segmentCollection,currentSegment,progress,offset);

                //TRS
                translation.Value = CollisionSystem.CalculateCarPosition(segmentCollection,currentSegment,progress,offset);
                rotation.Value = quaternion.LookRotationSafe(forward, new float3(0,1,0));
                scale.Value = size.GetSize();

            }).ScheduleParallel();
    }
}