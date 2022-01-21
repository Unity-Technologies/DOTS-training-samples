using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public struct GridSpawnerData : IComponentData
{
    public Entity PrefabToSpawn;
    public int SpawnedCount;
    public Random Random;
    public float3 Position;
}