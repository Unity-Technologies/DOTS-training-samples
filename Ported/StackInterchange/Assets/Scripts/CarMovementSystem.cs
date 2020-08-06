using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateAfter(typeof(CarInitializeSystem))]
public class CarMovementSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
       SegmentCollection segmentCollection = GetSingleton<SegmentCollection>();

        var deltaTime = Time.DeltaTime;

        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        // We could assume that we have only linear segments of the same length to optimize
        // by removing the currentSegment lookup
        Entities.ForEach((
            Entity entity,
            int entityInQueryIndex,
            ref Progress progress,
            in CurrentSegment currentSegment,
            in Speed speed) =>
        {
            var segment = segmentCollection.Value.Value.Segments[currentSegment.Value];

            var start = segment.Start;
            var end = segment.End;

            // We use the square distance here to avoid calculating the square root.
            // Could also calculate this once and keep it at the SegmentData struct?
            // Could also avoid this is if we assume that all segments have the same length and the segments are always linear
            var segmentLength = math.distancesq(start, end);

            var dx = deltaTime * speed.Value * 10f;

            progress.Value += dx / segmentLength;
            progress.Value = math.clamp(progress.Value, 0f, 1f);

            // Car has finished, add a tag so that it can be picked by another system
            if (progress.Value == 1f)
            {
                commandBuffer.AddComponent<Finished>(entityInQueryIndex, entity);
            }
        })
        .Schedule();
    }
}
