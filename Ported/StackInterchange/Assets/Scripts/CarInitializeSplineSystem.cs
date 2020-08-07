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
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    Random _random;

    protected override void OnCreate()
    {
        splineQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Spline>(),
                ComponentType.ReadOnly<SplineCategory>()
            }
        });
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

        _random = new Random(898934);
    }

    protected override void OnUpdate()
    {
        var random = new Random((uint)(56418f * Time.DeltaTime));

        var splineData = splineQuery.ToComponentDataArray<Spline>(Allocator.TempJob);
        var splineCategories = splineQuery.ToComponentDataArray<SplineCategory>(Allocator.TempJob);

        Entities.
            WithAll<Disabled>().
            WithNone<Restart>().
            WithName("CanInitializeSpline").
            ForEach((
                int entityInQueryIndex,
                ref BelongToSpline belongToSpline,
                ref CurrentSegment currentSegment,
                ref SegmentCounter segmentCounter,
                ref URPMaterialPropertyBaseColor color) =>
            {
                //Spline and segment
                var randomSplineId = random.NextInt(0, splineData.Length);
                var spline = splineData[randomSplineId];

                var segmentIndex = random.NextInt(0, spline.Value.Value.Segments.Length);

                belongToSpline.Value = randomSplineId;
                currentSegment.Value = spline.Value.Value.Segments[segmentIndex];
                segmentCounter.Value = segmentIndex;

                color.Value = splineCategories[randomSplineId].GetColor();

            }).ScheduleParallel();

        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.
            WithAll<Disabled>().
            WithAll<Restart>().
            WithName("CanInitializeSpline_Restart_Disabled").
            WithDisposeOnCompletion(splineData).
            WithDisposeOnCompletion(splineCategories).
            ForEach((
                Entity entity,
                int entityInQueryIndex,
                ref BelongToSpline belongToSpline,
                ref CurrentSegment currentSegment,
                ref SegmentCounter segmentCounter,
                ref URPMaterialPropertyBaseColor color) =>
            {
                //Spline and segment
                var randomSplineId = random.NextInt(0, splineData.Length);
                belongToSpline.Value = randomSplineId;

                var spline = splineData[randomSplineId];

                var segmentIndex = 0;
                currentSegment.Value = spline.Value.Value.Segments[segmentIndex];
                segmentCounter.Value = segmentIndex;

                color.Value = splineCategories[randomSplineId].GetColor();

                commandBuffer.RemoveComponent<Restart>(entityInQueryIndex, entity);
            }).ScheduleParallel();

        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
