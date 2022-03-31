using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using static BucketBrigadeUtility;

[UpdateAfter(typeof(IdleSystem))]
public partial class BucketDetectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var helperEntity = GetSingletonEntity<FreeBucketInfo>();
        var bucketBuffer = EntityManager.GetBuffer<FreeBucketInfo>(helperEntity);
        var frame = GetCurrentFrame();
        
        bucketBuffer.Clear();
        
        Entities.WithAll<BucketTag>()
            .WithName("BucketInfoCollect")
            .ForEach((in Entity entity, in MyBucketState state, in Position position) =>
            {
                // do not scoop buckets on same frame -- too many race conditions there.
                if (frame > state.FrameChanged + 2)
                {
                    switch (state.Value)
                    {
                        case BucketState.EmptyOnGround:
                        case BucketState.FullOnGround:
                            ecb.AppendToBuffer(helperEntity,
                                new FreeBucketInfo()
                                    { BucketEntity = entity, BucketPosition = position, BucketState = state });
                            break;
                    }
                }
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
        
        bucketBuffer = EntityManager.GetBuffer<FreeBucketInfo>(helperEntity);
        
        Entities.WithReadOnly(bucketBuffer)
            .WithName("BucketDetection")
            .WithAny<FetcherTag, OmniworkerTag>()
            .ForEach((ref MyWorkerState state, ref RelocatePosition target, ref BucketToWant bucketToWant, in Position position) =>
            {
                if (state.Value == WorkerState.BucketDetection)
                {
                    (var bucket, var bucketPosition) = FindClosestBucket(position.Value, bucketBuffer, true);

                    if (bucket != Entity.Null)
                    {
                        if (IsVeryClose(position.Value, bucketPosition))
                        {
                            bucketToWant.Value = bucket;
                        }
                        else
                        {
                            target.Value = bucketPosition;
                        }
                    }

                    state.Value = WorkerState.Idle;
                }
            }).ScheduleParallel();
    }
}
