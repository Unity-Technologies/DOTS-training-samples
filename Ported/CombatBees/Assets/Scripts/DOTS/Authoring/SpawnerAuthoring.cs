using Unity.Entities;

[GenerateAuthoringComponent]
public struct Spawner : IComponentData
{
    public Entity Prefab;
    public int Count;
}