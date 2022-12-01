using Unity.Entities;

struct Config : IComponentData
{
    public Entity CommuterPrefab;

    public Entity PlatformPrefab;
}