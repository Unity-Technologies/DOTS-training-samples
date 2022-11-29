using Unity.Entities;
using Unity.Mathematics;

public struct Walkway : IComponentData
{
    public float3 LowPoint;
    public float3 HighPoint;
}