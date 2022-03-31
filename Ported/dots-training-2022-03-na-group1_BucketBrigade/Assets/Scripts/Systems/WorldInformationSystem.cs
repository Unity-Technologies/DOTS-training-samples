using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using static BucketBrigadeUtility;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
[UpdateAfter(typeof(FrameInitializationSystem))]
public partial class WorldInformationSystem : SystemBase
{
    static float2 CalculateLeftArc(float2 a, float2 b, float t)
    {
        var ab = b - a;

        return a + (ab * t) + (new float2(-ab.y, ab.x) * ((1f - t) * t * 0.3f));
    }

    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        var currentFrame = GetCurrentFrame();
        

        var bucketBufferEntity = GetSingletonEntity<FreeBucketInfo>();
        var bucketECB = new EntityCommandBuffer(Allocator.TempJob);
        var bucketECBParallel = bucketECB.AsParallelWriter();

        Entities.WithAll<BucketTag>()
            .WithName("BucketInfoCollect")
            .ForEach((int entityInQueryIndex, in Entity entity, in MyBucketState state, in Position position) =>
            {
                // do not scoop buckets on same frame -- too many race conditions there.
                if (currentFrame > state.FrameChanged + 2)
                {
                    switch (state.Value)
                    {
                        case BucketState.EmptyOnGround:
                        case BucketState.FullOnGround:
                            bucketECBParallel.AppendToBuffer(entityInQueryIndex, bucketBufferEntity,
                                new FreeBucketInfo()
                                    { BucketEntity = entity, BucketPosition = position, BucketState = state });
                            break;
                    }
                }
            }).ScheduleParallel();
        
        var waterPoolBufferEntity = GetSingletonEntity<WaterPoolInfo>();
        var waterECB = new EntityCommandBuffer(Allocator.TempJob);
        var waterECBParallel = waterECB.AsParallelWriter();
        
        Entities.WithName("WaterInfoCollect")
            .WithNone<BucketTag>()
            .ForEach((int entityInQueryIndex, in Entity entity, in Volume volume, in Position position, in Scale scale) =>
            {
                // water pool only counts if it has volume.
                if (volume.Value > 0.01)
                {
                    waterECBParallel.AppendToBuffer(entityInQueryIndex, waterPoolBufferEntity, new WaterPoolInfo() { WaterPool = entity, Position = position.Value, Radius = scale.Value.x / 2f});
                }
            }).ScheduleParallel();
        
        CompleteDependency();
        
        bucketECB.Playback(EntityManager);
        bucketECB.Dispose();
        waterECB.Playback(EntityManager);
        waterECB.Dispose();
    }
}
