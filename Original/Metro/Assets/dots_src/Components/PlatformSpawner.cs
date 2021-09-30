using Unity.Entities;

[GenerateAuthoringComponent]
public struct PlatformSpawner : IComponentData
{
    public Entity LinePrefab;
    public Entity PlatformPrefab;
}