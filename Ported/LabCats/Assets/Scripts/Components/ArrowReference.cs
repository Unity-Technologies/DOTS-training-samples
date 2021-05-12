using Unity.Entities;

[InternalBufferCapacity(3)]
public struct ArrowReference : IBufferElementData
{
    public Entity Value;
}

public struct NextArrowIndex : IComponentData
{
    public int Value;
}