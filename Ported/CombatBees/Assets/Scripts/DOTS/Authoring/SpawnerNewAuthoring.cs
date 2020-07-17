using Unity.Entities;

[GenerateAuthoringComponent]
public struct SpawnerNew : IComponentData
{
    public Entity Prefab;
    public int Count;
}