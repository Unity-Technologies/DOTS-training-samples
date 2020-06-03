using Unity.Entities;

[GenerateAuthoringComponent]
public struct SpawnerInfo : IComponentData
{
    public Entity Prefab;
    public float Frequency;
}