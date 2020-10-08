using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct MyNonUniformScale : IComponentData
{
    public float3 Value;
}