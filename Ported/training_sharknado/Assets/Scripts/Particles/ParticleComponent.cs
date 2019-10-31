using System;
using Unity.Entities;

// Tornado Particles tag
[Serializable]
public struct ParticleComponent : IComponentData
{
    public float RadiusMult;
}