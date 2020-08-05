using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

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
                //Get segment
                var segment = segmentCollection.Value.Value.Segments[currentSegment.Value];

                //TRS
                var position = math.lerp(segment.Start,segment.End,progress.Value);
                var forward = segment.End - position;
                translation.Value = position;
                rotation.Value = quaternion.LookRotationSafe(forward,new float3(0,1,0));
                scale.Value = size.Value;

            }).ScheduleParallel();
    }
}