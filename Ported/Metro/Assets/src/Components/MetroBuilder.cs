using Unity.Entities;

public struct MetroBuilder : IComponentData
{
    public Entity CarriagePrefab;
    public Entity PlatformPrefab;
    public Entity CommuterPrefab;
    public Entity RailPrefab;
}
