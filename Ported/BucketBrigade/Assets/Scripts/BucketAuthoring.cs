using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Bucket : IComponentData
{
    public float4 fullColor;
    public float4 emptyColor;
}