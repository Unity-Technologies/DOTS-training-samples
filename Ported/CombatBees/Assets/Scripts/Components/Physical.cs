using Unity.Entities;
using Unity.Mathematics;

public struct Physical : IComponentData
{
    public float3 Position;
    public float3 Velocity;
}