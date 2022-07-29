using Unity.Entities;

// An empty component is called a "tag component".
struct WaterBringer : IComponentData
{
    public enum WaterBringerState
    {
        GoToIdle,
        GoToFullBucket,
        GoToNextFireFighter
    };
    public WaterBringerState State;
}