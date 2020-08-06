using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateAfter(typeof(CarSpawnerSystem))]
public class CarInitializeSplineSystem : SystemBase
{
    private EntityQuery splineQuery;
    Random _random;

    protected override void OnCreate()
    {
        _random = new Random( (uint) 56418);
        splineQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Spline>()
            }
        });
    }

    protected override void OnUpdate()
    {
        var random = _random;

        var splineEntities = splineQuery.ToEntityArrayAsync(Allocator.TempJob, out var splineEntitiesHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, splineEntitiesHandle);

        Entities.
            WithAll<Disabled>().
            WithName("CanInitializeSpline").
            WithDisposeOnCompletion(splineEntities).
            ForEach((
            ref BelongToSpline belongToSpline,
            ref CurrentSegment currentSegment,
            ref SegmentCounter segmentCounter,
            ref URPMaterialPropertyBaseColor color) =>
            {
                //Spline and segment
                int randomSplineId = random.NextInt(0, splineEntities.Length);

                belongToSpline.Value = splineEntities[randomSplineId];
                var splineData = GetComponent<Spline>(splineEntities[randomSplineId]);
                currentSegment.Value = splineData.Value.Value.Segments[0];
                segmentCounter.Value = 0;

                color.Value = new float4(splineData.color.x, splineData.color.y, splineData.color.z, 1.0f);
            }).ScheduleParallel();
    }
}
