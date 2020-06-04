using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[MaterialProperty("_CubeColor", MaterialPropertyFormat.Float4)]
public struct CubeColor : IComponentData
{
    public float4 Color;
}