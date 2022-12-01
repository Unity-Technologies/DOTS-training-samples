using Unity.Entities;
using Unity.Mathematics;

public struct Hive : IComponentData
{
    public float4 color;
    public float3 boundsExtents;
    public float3 boundsPosition;
}

public struct TargetBee : IBufferElementData
{
    public Entity enemy;
    public float3 position;
}

public struct AvailableResources : IBufferElementData
{
    public Entity resource;
    public float3 position;
}
