using Unity.Entities;

[GenerateAuthoringComponent]
public struct ParticleSpawnerAuthoring : IComponentData
{
    public Entity BloodPrefab;
    public Entity SmokePrefab; 
}