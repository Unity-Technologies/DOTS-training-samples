
using Unity.Entities;

public struct TeamSpawnerComponent : IComponentData
{
    public Entity WorkerPrefab;
    public int NumberOfTeams;
}