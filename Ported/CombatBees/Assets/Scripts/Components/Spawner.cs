using Unity.Entities;

[GenerateAuthoringComponent]
public struct Spawner : IComponentData
{
    public Entity BeePrefab;
    public Entity BloodPrefab;
    public Entity ResourcePrefab;
}