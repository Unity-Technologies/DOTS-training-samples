using Unity.Entities;

[InternalBufferCapacity(3)]
public struct ArrowReference : IBufferElementData
{
    public Entity Entity;
}
