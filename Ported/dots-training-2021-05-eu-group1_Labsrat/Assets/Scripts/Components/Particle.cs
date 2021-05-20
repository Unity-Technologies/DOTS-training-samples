using Unity.Entities;
using Unity.Mathematics;

public struct ParticleVelocity : IComponentData
{
    public float3 Value;
}

public struct ParticleAge : IComponentData
{
    public float Value;
}

public struct ParticleLifetime : IComponentData
{
    public float Value;
}

