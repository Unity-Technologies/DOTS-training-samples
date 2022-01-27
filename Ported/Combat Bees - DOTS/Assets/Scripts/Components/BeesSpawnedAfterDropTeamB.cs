using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

[GenerateAuthoringComponent]
public struct BeesSpawnedAfterDropTeamB : IComponentData
{
    public Entity PrefabToSpawn;
    public int SpawnedCount;
    public Random Random;
    public float MaxInitVelocity;
}
