using Unity.Entities;

// An empty component is called a "tag component".
struct BucketBringer : IComponentData
{
    public enum BucketBringerState
    {
        GoToIdle,
        GoToEmptyBucket,
        GoToNextFireFighter
    };
    public BucketBringerState State;
}