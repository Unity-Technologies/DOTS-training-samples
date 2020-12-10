using System;
using Unity.Entities;
using UnityEngine;

public struct ParticleSpawner : IComponentData
{
    public Entity ParticlePrefab;
    public int ParticleCount;
}
