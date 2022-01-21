using Unity.Entities;
using Unity.Mathematics;

public struct Container : IComponentData
{
    public float3 Center;
    public float3 Dimensions;
    public float3 MinPosition;
    public float3 MaxPosition;
}
