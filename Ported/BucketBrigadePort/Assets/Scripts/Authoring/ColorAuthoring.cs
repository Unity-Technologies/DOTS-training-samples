using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Color : IComponentData
{
    public float4 Value;
}
