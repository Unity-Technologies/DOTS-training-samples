using Unity.Entities;
using Unity.Mathematics;

public struct Bounds : IComponentData
{
    public float3 Center;
    public float3 Extents;

    public float3 Size => Extents * 2;
    public float3 Min => Center - Extents;
    public float3 Max => Center + Extents; 
}
