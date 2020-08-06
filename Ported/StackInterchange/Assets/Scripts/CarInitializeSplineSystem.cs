using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[UpdateAfter(typeof(CarSpawnerSystem))]
public class CarInitializeSplineSystem : SystemBase
{
    private EntityQuery splineQuery;
    Random _random;

    protected override void OnCreate()
    {
        _random = new Random((uint)56418);
        splineQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Spline>(),
                ComponentType.ReadOnly<SplineCategory>()
            }
        });
    }

    protected override void OnUpdate()
    {
        var random = _random;
        
        //var splineEntities = splineQuery.ToEntityArrayAsync(Allocator.TempJob, out var splineEntitiesHandle);
        //Dependency = JobHandle.CombineDependencies(splineEntitiesHandle, Dependency);
        var splineDatas = splineQuery.ToComponentDataArray<Spline>(Allocator.TempJob);
        var splineCategories = splineQuery.ToComponentDataArray<SplineCategory>(Allocator.TempJob);
        //var splineDebugData = splineQuery.ToComponentDataArray<Spline>(Allocator.TempJob);

        Entities.
            WithAll<Disabled>().
            WithName("CanInitializeSpline").
            WithoutBurst().
            //WithDisposeOnCompletion(splineEntities).
            ForEach((
                ref BelongToSpline belongToSpline,
                ref CurrentSegment currentSegment,
                ref SegmentCounter segmentCounter,
                ref URPMaterialPropertyBaseColor color) =>
            {
                //Spline and segment
                var randomSplineId = random.NextInt(0, splineDatas.Length);

                belongToSpline.Value = randomSplineId;
                //var splineData = GetComponent<Spline>(splineEntities[randomSplineId]);
                var splineData = splineDatas[randomSplineId];
                currentSegment.Value = splineData.Value.Value.Segments[0];
                segmentCounter.Value = 0;

                color.Value = splineCategories[randomSplineId].GetColor();
            }).Run();
        
        splineDatas.Dispose();
        splineCategories.Dispose();

        _random = random; 

        Dependency.Complete();
    }
}
