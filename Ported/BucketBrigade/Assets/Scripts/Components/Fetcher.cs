using Unity.Entities;
using Unity.Mathematics;

public enum FetcherState
{
    Idle,
    MoveTowardsBucket,
    ArriveAtBucket,
    // TODO: PickUpBucket?
    MoveTowardsWater,
    ArriveAtWater,
    FillingBucket
}

struct Fetcher : IComponentData
{
    public FetcherState CurrentState;
    
    public Entity TargetPickUp;
    public Entity TargetDropZone;
    
    public float SpeedFull;
    public float SpeedEmpty;
}