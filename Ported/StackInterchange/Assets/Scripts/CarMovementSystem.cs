using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateAfter(typeof(CollisionSystem))]
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
            //1. compute them from source start/end
            //var segment = segmentCollection.Value.Value.Segments[currentSegment.Value];
            //var start = segment.Start;
            //var end = segment.End;
            //var segmentLength = math.distance(start, end);

            //2. Better 1ms to 0.5ms
            var segmentLength = segmentCollection.Value.Value.SegmentsLength[currentSegment.Value];

            var dx = deltaTime * speed.Value;

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
