using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(32)]
public struct PickupBucketCommand : IBufferElementData
{
    public Entity Worker;
    public Entity Bucket;
    public int Delay;
    
    public PickupBucketCommand(Entity worker, Entity bucket, int delay = 0)
    {
        Worker = worker;
        Bucket = bucket;
        Delay = delay;
    }
}