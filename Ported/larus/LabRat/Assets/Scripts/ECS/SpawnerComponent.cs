using Unity.Entities;

public struct SpawnerComponent : IComponentData
{
    public Entity Prefab;
    public Entity AlternatePrefab;
    public float Max;
    public float Frequency;
    public float Counter;
    public int TotalSpawned;
    public bool InAlternate;
    public float Timer;
}