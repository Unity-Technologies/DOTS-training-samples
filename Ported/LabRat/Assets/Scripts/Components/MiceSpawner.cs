using Unity.Entities;

public struct MiceSpawner : IComponentData
{
    public float SpawnCounter;
    public int RemainingMiceToSpawn;
}
