using Unity.Entities;
using Unity.Mathematics;

public enum FetcherState
{
    Idle,
    FetchingBucket,
    MoveTowardsWater,
    FillingBucket
}

struct Fetcher : IComponentData
{
    public FetcherState CurrentState;
    
    public Entity TargetPickUp;
    public Entity TargetDropZone;
    
    public float3 Speed;
}