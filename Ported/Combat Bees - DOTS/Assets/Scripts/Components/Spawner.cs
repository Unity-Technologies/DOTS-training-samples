using Unity.Entities;
using Random = Unity.Mathematics.Random;

public struct Spawner : IComponentData
{
    public Entity PrefabToSpawn;
    public int SpawnedCount;
    public Random Random;
}