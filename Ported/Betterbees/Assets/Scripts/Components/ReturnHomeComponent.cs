using Unity.Entities;
using Unity.Mathematics;

public struct ReturnHomeComponent : IComponentData
{
    public float3 HomeMinBounds;
    public float3 HomeMaxBounds;
}
