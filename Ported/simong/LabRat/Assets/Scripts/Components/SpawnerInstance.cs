using Unity.Entities;

[GenerateAuthoringComponent]
public struct SpawnerInstance : IComponentData
{
    public float TotalSpawned;
    public float Time;
    public float AlternateSpawnTime;
    public float CurrentAlternateSpawnFrenquency;
}