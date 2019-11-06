using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[GenerateAuthoringComponent]
[MaterialProperty("_Color", MaterialPropertyFormat.Float4)]
public struct MaterialColorComponent : IComponentData
{
    public float4 Value;
}
