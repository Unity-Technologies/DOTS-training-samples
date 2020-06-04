using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[MaterialProperty("_CubeColor", MaterialPropertyFormat.Float3)]
public struct CubeColor : IComponentData
{
    public float3 Color;
}