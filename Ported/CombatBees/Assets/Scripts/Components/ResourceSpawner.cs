using Unity.Entities;

public struct ResourceSpawner : IComponentData
{
    public Entity ResourcePrefab;
    public int StartingResourceCount;
}