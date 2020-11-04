using Unity.Entities;

[GenerateAuthoringComponent]
public struct MetroData : IComponentData
{
    public Entity PlatformPrefab;
    public Entity RailPrefab;
    public Entity CommuterPrefab;
}
