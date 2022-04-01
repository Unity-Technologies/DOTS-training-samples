using Unity.Entities;

[GenerateAuthoringComponent]
public struct ResourceSpawner : IComponentData
{
    public Entity ResourcePrefab;
    public int StartingResourceCount;
}