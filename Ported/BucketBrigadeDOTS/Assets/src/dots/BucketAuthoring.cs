using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

public struct WaterBucket : IComponentData
{
    public float Value;
}

[MaterialProperty("_BaseColor", MaterialPropertyFormat.Float4)]
public struct BaseColor : IComponentData
{
    public float4 Value;
}