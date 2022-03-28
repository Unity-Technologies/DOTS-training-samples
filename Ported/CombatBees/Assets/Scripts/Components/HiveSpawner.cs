using Unity.Entities;

public struct HiveSpawner : IComponentData
{
    public Entity BeePrefab;
    public int BeesAmount;
    public Entity ResourcePrefab;
    public int ResourceAmount;
}
