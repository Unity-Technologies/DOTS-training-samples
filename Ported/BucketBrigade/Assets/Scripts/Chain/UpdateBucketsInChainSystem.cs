using Unity.Entities;
using Unity.Collections;

[UpdateBefore(typeof(UpdateObjectsInChainSystem))]
public class UpdateBucketsInChainSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<BucketInChain>();
    }

    protected override void OnUpdate()
    {
        var bucketsArrayEntity = GetSingletonEntity<BucketInChain>();
        var bucketsArray = EntityManager.GetBuffer<BucketInChain>(bucketsArrayEntity);

        using (var ecb = new EntityCommandBuffer(Allocator.TempJob))
        {
            Entities
                .WithName("UpdateBucketsInChain")
                .ForEach(
                    (Entity entity, int entityInQueryIndex,
                        in UpdateBucketInChain bucket) =>
                    {
                        var newBucket = bucketsArray[bucket.index];
                        newBucket.bucketPos = bucket.bucketPos;
                        newBucket.bucketShift = bucket.bucketShift;
                        bucketsArray[bucket.index] = newBucket;
                        ecb.DestroyEntity(entity);
                    })
                .Run();
            ecb.Playback(EntityManager);
        }
    }
}