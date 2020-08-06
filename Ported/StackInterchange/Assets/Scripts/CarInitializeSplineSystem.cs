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

    protected override void OnCreate()
    {
        splineQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Spline>()
            }
        });
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var random = new Random((uint) (56418f * Time.DeltaTime));

        var splineEntities = splineQuery.ToEntityArrayAsync(Allocator.TempJob, out var splineEntitiesHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, splineEntitiesHandle);

        Entities.
            WithAll<Disabled>().
            WithNone<Restart>().
            WithName("CanInitializeSpline_Start_Disabled").
            ForEach((
            ref BelongToSpline belongToSpline,
            ref CurrentSegment currentSegment,
            ref SegmentCounter segmentCounter,
            ref URPMaterialPropertyBaseColor color) =>
            {
                //Spline and segment
                var randomSplineId = random.NextInt(0, splineEntities.Length);

                belongToSpline.Value = splineEntities[randomSplineId];
                var splineData = GetComponent<Spline>(splineEntities[randomSplineId]);

                var segmentIndex = random.NextInt(0, splineData.Value.Value.Segments.Length);
                currentSegment.Value = splineData.Value.Value.Segments[segmentIndex];
                segmentCounter.Value = segmentIndex;
                color.Value = GetComponent<SplineCategory>(splineEntities[randomSplineId]).GetColor();
            }).ScheduleParallel();

        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.
        WithAll<Disabled>().
        WithAll<Restart>().
        WithName("CanInitializeSpline_Restart_Disabled").
        WithDisposeOnCompletion(splineEntities).
        ForEach((
        Entity entity,
        int entityInQueryIndex,
        ref BelongToSpline belongToSpline,
        ref CurrentSegment currentSegment,
        ref SegmentCounter segmentCounter,
        ref URPMaterialPropertyBaseColor color) =>
        {
            //Spline and segment
            var randomSplineId = random.NextInt(0, splineEntities.Length);

            belongToSpline.Value = splineEntities[randomSplineId];
            var splineData = GetComponent<Spline>(splineEntities[randomSplineId]);

            var segmentIndex = 0;
            currentSegment.Value = splineData.Value.Value.Segments[segmentIndex];
            segmentCounter.Value = segmentIndex;
            color.Value = GetComponent<SplineCategory>(splineEntities[randomSplineId]).GetColor();

            commandBuffer.RemoveComponent<Restart>(entityInQueryIndex, entity);

        }).ScheduleParallel();

        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
