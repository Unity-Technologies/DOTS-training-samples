using Unity.Entities;

public struct RailsSpawner : IComponentData
{
    public Entity RailPrefab;
    public Entity PlatformPrefab;
    public int NbRails;
}