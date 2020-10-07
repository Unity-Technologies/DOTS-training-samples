using Unity.Entities;

[GenerateAuthoringComponent]
public struct SpawnPoint : IComponentData
{
    public Entity spawnPrefab;
    public byte direction;
    public int spawnCount;
    public float spawnFrequency;
}
