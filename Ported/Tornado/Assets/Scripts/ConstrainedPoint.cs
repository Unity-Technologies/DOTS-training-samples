using Unity.Entities;
using Unity.Mathematics;

public struct ConstrainedPoint : IComponentData
{
    public float3 position;
    public float3 oldPosition;
    public int neightbours;
    public bool anchor;
}

public struct ConstrainedPointEntry : IBufferElementData
{
    public ConstrainedPoint Value;
}
