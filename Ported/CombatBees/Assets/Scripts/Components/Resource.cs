using Unity.Entities;
using Unity.Mathematics;

public struct Resource : IComponentData
{
    public float3 Position;
    public float3 Velocity;
    public bool Dead;
}