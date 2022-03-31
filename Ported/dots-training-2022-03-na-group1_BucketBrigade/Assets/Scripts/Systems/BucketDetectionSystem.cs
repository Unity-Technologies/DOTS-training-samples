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
        var helperEntity = GetSingletonEntity<FreeBucketInfo>();
        var bucketBuffer = EntityManager.GetBuffer<FreeBucketInfo>(helperEntity);

        var pickupBucketCommandEntity = GetSingletonEntity<PickupBucketCommand>();
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var ecbParallel = ecb.AsParallelWriter();
        
        Entities.WithReadOnly(bucketBuffer)
            .WithName("BucketDetection")
            .WithAny<FetcherTag, OmniworkerTag>()
            .ForEach((int entityInQueryIndex, Entity entity, ref MyWorkerState state, ref RelocatePosition target, in Position position) =>
            {
                if (state.Value == WorkerState.BucketDetection)
                {
                    (var bucket, var bucketPosition) = FindClosestBucket(position.Value, bucketBuffer, true);

                    if (bucket != Entity.Null)
                    {
                        if (IsVeryClose(position.Value, bucketPosition))
                        {
                            ecbParallel.AppendToBuffer(entityInQueryIndex, pickupBucketCommandEntity, new PickupBucketCommand(entity, bucket));
                        }
                        else
                        {
                            target.Value = bucketPosition;
                        }
                    }

                    state.Value = WorkerState.Idle;
                }
            }).ScheduleParallel();
        
        CompleteDependency();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
