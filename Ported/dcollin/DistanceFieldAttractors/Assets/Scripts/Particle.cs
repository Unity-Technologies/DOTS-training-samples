using Unity.Entities;
using Unity.Mathematics;

public struct ParticlePosition : IComponentData
{
    public float3 value;
}
public struct ParticleVelocity : IComponentData
{
    public float3 value;
}
public struct ParticleColor : IComponentData
{
    public float4 value;
}
