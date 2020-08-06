using System.ComponentModel;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(CarMovementSystem))]
public class CarSegmentChangeSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.
            WithName("CarSegmentChange").
            WithAll<Finished>().ForEach((
            Entity entity,
            int entityInQueryIndex,
            ref CurrentSegment segment,
            ref SegmentCounter counter,
            ref Progress progress,
            in BelongToSpline spline) =>
        {
            //Spline and segment
            var splineData = GetComponent<Spline>(spline.Value);

            // Reached the end, need to re-Init to another spline
            if (counter.Value == splineData.Value.Value.Segments.Length - 1)
            {
                commandBuffer.AddComponent<Disabled>(entityInQueryIndex, entity);
                commandBuffer.AddComponent<Restart>(entityInQueryIndex, entity);
            }
            else
            {
                // Just change the spline and set progress to 0
                segment.Value = splineData.Value.Value.Segments[++counter.Value];
                progress.Value = 0f;
            }

            commandBuffer.RemoveComponent<Finished>(entityInQueryIndex, entity);

        }).ScheduleParallel();

        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
