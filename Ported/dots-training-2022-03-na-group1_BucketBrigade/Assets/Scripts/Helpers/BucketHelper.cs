using Unity.Entities;

static class BucketHelper
{
    public static void SetState(EntityManager manager, Entity bucket, BucketState state)
    {
        var bucketState = manager.GetComponentData<MyBucketState>(bucket);
        bucketState.Value = state;
        manager.SetComponentData(bucket, bucketState);
    }

}
