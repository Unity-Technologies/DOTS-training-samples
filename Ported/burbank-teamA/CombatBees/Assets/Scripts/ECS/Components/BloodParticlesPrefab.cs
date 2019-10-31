using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct BloodParticlesPrefab : IComponentData
{
    public Entity Value;
}
