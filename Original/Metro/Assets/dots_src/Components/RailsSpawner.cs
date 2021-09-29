using Unity.Entities;

public struct RailsSpawner : IComponentData
{
    public Entity RailPrefab;
    public int NbRails;
}