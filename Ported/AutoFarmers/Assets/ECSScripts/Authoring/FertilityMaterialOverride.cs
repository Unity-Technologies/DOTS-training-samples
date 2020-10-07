using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[MaterialProperty("_BaseColor", MaterialPropertyFormat.Float4)]
public struct FertilityMaterialOverride : IComponentData
{
    public float4 BaseColor;
}
