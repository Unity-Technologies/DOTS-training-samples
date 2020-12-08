using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct ParticleSpawner : IComponentData
{
    public Entity ParticlePrefab;
    public int ParticleCount;
}
