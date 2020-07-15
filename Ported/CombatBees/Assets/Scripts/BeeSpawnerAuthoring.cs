using Unity.Entities;

[GenerateAuthoringComponent]
public struct BeeSpawner : IComponentData
{
    public Entity Prefab;
    public int BeeCount;
}