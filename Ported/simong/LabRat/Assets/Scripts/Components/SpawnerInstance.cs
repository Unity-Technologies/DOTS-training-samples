using Unity.Entities;

[GenerateAuthoringComponent]
public struct SpawnerInstance : IComponentData
{
    public float Time;
    public float AlternateSpawnTime;
}