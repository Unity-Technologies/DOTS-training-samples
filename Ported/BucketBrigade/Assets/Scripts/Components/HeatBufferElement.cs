using Unity.Entities;

[InternalBufferCapacity(1000)]
public struct HeatBufferElement : IBufferElementData
{
    public float Heat;
}
