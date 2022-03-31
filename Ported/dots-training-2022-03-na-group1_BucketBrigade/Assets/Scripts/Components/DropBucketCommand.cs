using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(32)]
public struct DropBucketCommand : IBufferElementData
{
    public Entity Worker;
    
    public DropBucketCommand(Entity worker)
    {
        Worker = worker;
    }
}