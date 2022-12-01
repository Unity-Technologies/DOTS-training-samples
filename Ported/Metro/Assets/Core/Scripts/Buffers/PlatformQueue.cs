using Unity.Entities;

[InternalBufferCapacity(9)]
public struct PlatformQueue : IBufferElementData
{
    public DynamicBuffer<QueueItem> queue;
}
