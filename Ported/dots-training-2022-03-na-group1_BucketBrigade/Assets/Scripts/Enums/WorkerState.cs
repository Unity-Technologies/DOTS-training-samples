using Unity.Entities;
using Unity.Mathematics;

public enum WorkerState
{
    Idle,
    Repositioning,
    FillingBucket,
    BucketDetection,
}
