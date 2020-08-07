using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[GenerateAuthoringComponent]
[MaterialProperty("_BaseColor", MaterialPropertyFormat.Float4)]
public struct AntColor : IComponentData
{
    public float4 value;
}