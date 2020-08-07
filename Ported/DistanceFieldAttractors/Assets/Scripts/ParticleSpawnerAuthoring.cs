using Unity.Entities;

[GenerateAuthoringComponent]
public struct ParticleSpawner : IComponentData
{
    public Entity Prefab;
    public int SpawnCount;
}
