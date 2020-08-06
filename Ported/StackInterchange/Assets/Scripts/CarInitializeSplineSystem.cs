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

                int destinationType = randomSplineId % 4; //repeat the colors for now
                switch (destinationType)
                {
                    case 0: color.Value = new float4(1, 0, 0, 1); break; //Red
                    case 1: color.Value = new float4(0, 0, 1, 1); break; //Blue
                    case 2: color.Value = new float4(0.5f, 0, 1, 1); break; //Purple
                    case 3: color.Value = new float4(1, 0.8f, 1, 1); break; //Pink
                }
            }).ScheduleParallel();
    }
}
