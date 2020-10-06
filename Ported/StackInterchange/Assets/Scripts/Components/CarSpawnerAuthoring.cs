using Unity.Entities;

[GenerateAuthoringComponent]
public struct CarSpawner : IComponentData
{
    public Entity CarPrefab; // TODO: Multiple sizes?
    public Translation Position;
    public SpawnerFrequency SpawnerFrequency;
    // Orientation
    // public Color[] Colors; // ???
}