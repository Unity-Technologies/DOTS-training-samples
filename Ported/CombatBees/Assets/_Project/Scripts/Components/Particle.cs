using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct ParticleVelocity : IComponentData
{
    public float3 Velocity;
}

[Serializable]
public struct ParticleDrag : IComponentData
{
    public float Drag;
}

[Serializable]
public struct ParticleLifetime : IComponentData
{
    public float MaxLifetime;
    public float Lifetime;
}

[Serializable]
public struct ParticleOrientTowardsVelocity : IComponentData
{
}

[Serializable]
public struct ParticleSize : IComponentData
{
    public float3 Size;
}

[Serializable]
public struct ParticleShrinkOverLifetime : IComponentData
{
}

[Serializable]
public struct ParticleStretchWithVelocity : IComponentData
{
    public float Factor;   
    public float Max;   
}

[Serializable]
public struct ParticleGravity : IComponentData
{
    public float3 Value;   
}

[Serializable]
public struct ParticleBloodInFlight : IComponentData
{
}