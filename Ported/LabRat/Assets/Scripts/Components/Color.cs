using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[MaterialProperty("_BaseColor", MaterialPropertyFormat.Float4)]
public struct LabRat_Color : IComponentData
{
    public float4 Value;
}
