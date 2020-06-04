using Unity.Entities;

[GenerateAuthoringComponent]
public struct SpawnerInfo : IComponentData
{
    public Entity Prefab;
    public Entity AlternatePrefab;
    public float Frequency;
    public float AlternateSpawnMinFrequency;
    public float AlternateSpawnMaxFrequency;
}