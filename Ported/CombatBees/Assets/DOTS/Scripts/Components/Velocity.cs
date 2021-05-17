using Unity.Entities;
using Unity.Mathematics;

public struct Velocity : IComponentData
{
    public float3 Value;
}

public struct RandomVelocity : IComponentData
{
    public float3 Value;
}
