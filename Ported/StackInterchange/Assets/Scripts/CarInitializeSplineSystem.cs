using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(CarSpawnerSystem))]
public class CarInitializeSplineSystem : SystemBase
{
    private EntityQuery splineQuery;

    protected override void OnCreate()
    {
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
        var random = new Random((uint)Time.ElapsedTime + 18564584);

        var splineEntities = splineQuery.ToEntityArrayAsync(Allocator.TempJob, out var splineEntitiesHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, splineEntitiesHandle);

        Entities.
            WithAll<Disabled>().
            WithName("CanInitializeSpline").
            WithDisposeOnCompletion(splineEntities).
            ForEach((
            ref BelongToSpline belongToSpline,
            ref CurrentSegment currentSegment,
            ref SegmentCounter segmentCounter) =>
            {
                //Spline and segment
                int randomSplineId = random.NextInt(0, splineEntities.Length);
                belongToSpline.Value = splineEntities[randomSplineId];
                var splineData = GetComponent<Spline>(splineEntities[randomSplineId]);
                currentSegment.Value = splineData.Value.Value.Segments[0];
                segmentCounter.Value = 0;
            }).ScheduleParallel();
    }
}
