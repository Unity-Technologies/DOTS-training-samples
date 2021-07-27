using Unity.Entities;

public struct AntSpawner : IComponentData
{
    public Entity AntPrefab;
    public int AntCount;
}