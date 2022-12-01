using Unity.Entities;

[InternalBufferCapacity(16)]
public struct QueueItem : IBufferElementData
{
    public Entity commuter;
}
