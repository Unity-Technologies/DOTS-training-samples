using Unity.Entities;

[InternalBufferCapacity(50)]
public struct PlatformQueueBuffer : IBufferElementData
{
    public Entity Passenger;
}