using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Bounds : IComponentData
{
    public float2 BoundsCenter;
    public float2 BoundsExtent;
}