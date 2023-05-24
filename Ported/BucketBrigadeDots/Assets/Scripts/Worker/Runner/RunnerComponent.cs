using Unity.Entities;
using Unity.Mathematics;

public enum RunnerStates
{
    Idle,
    FetchingBucket,
    MovingBucket
}

public struct RunnerState : IComponentData
{
    public RunnerStates State;
    public float2 WaterPosition;
    public float2 TargetBucketPosition;
}