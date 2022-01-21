using System;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

[Serializable]
public struct RandomVelocitySpawnerData : IComponentData
{
    public Entity PrefabToSpawn;
    public int SpawnedCount;
    public Random Random;
    public float3 Position;
    public float MaxInitVelocity;
}
