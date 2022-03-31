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
