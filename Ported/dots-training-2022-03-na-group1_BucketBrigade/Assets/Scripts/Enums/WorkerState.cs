using Unity.Entities;
using Unity.Mathematics;

public enum WorkerState
{
    Thinking,
    RepositioningToWewLine,
    DeliveringBucketToPerson,
    DumpingBucket,
    FillingBucket,
    GoingToWaterSource,
    GoingToNearestFire,
    DroppingBucket,
    PickingBucketUp,
    GoingToNearestBucket,
    BringingEmptyBucketToWaterSource
}
