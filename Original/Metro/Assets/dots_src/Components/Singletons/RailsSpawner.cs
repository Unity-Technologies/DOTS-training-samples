using Unity.Entities;

[GenerateAuthoringComponent]
public struct RailsSpawner : IComponentData
{
    public Entity RailPrefab;
}