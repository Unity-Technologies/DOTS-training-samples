using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct ParticleComponent : IComponentData
{
    public enum ParticleType
    {
        Blood,
        SpawnFlash
    };

    public ParticleType Type;
}
