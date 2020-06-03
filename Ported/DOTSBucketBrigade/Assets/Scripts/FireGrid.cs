using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[InternalBufferCapacity(2500)]
public struct FireGridCell : IBufferElementData
{
    public float Temperature;
}

public struct FireGrid : IComponentData
{
    
}
