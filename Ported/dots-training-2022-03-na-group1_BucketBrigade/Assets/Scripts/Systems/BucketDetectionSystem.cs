using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class BucketDetectionSystem : SystemBase
{
    static (Entity entity, float2 position) FindClosestBucket(float2 position, DynamicBuffer<FreeBucketInfo> bucketInfo, bool mustBeEmpty)
    {
        var closest = Entity.Null;
        var closestPosition = float2.zero;
        var distanceSq = float.PositiveInfinity;

        for (var i = 0; i < bucketInfo.Length; i++)
        {
            var element = bucketInfo[i];

            if (!mustBeEmpty || element.BucketState.Value == BucketState.EmptyOnGround)
            {
                var candidateDistanceSq = math.distancesq(position, element.BucketPosition.Value);

                if (candidateDistanceSq < distanceSq)
                {
                    distanceSq = candidateDistanceSq;
                    closest = element.BucketEntity;
                    closestPosition = element.BucketPosition.Value;
                }
            }
        }

        return (closest, closestPosition);
    }

    static bool IsWithinPickupRange(float2 a, float2 b)
    {
        return math.distancesq(a, b) < 0.01f;
    }
    
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var helperEntity = GetSingletonEntity<FreeBucketInfo>();
        var bucketBuffer = EntityManager.GetBuffer<FreeBucketInfo>(helperEntity);
        
        bucketBuffer.Clear();
        
        Entities.WithAll<BucketTag>()
            .ForEach((in Entity entity, in MyBucketState state, in Position position) =>
            {
                switch (state.Value)
                {
                    case BucketState.EmptyOnGround:
                    case BucketState.FullOnGround:
                        ecb.AppendToBuffer(helperEntity, new FreeBucketInfo() { BucketEntity = entity, BucketPosition = position, BucketState = state});
                        break;
                }
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
        
        bucketBuffer = EntityManager.GetBuffer<FreeBucketInfo>(helperEntity);
        
        Entities.WithReadOnly(bucketBuffer)
            .WithAny<FetcherTag, OmniworkerTag>()
            .ForEach((ref MyWorkerState state, ref RelocatePosition target, ref BucketToWant bucketToWant, in Position position) =>
            {
                if (state.Value == WorkerState.BucketDetection)
                {
                    (var bucket, var bucketPosition) = FindClosestBucket(position.Value, bucketBuffer, true);

                    if (bucket != Entity.Null)
                    {
                        if (IsWithinPickupRange(position.Value, bucketPosition))
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
