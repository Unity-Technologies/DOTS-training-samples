using Unity.Entities;
using Unity.Mathematics;

public struct FoodSpawnerComponent : IComponentData
{
    public int initialSpawnAmount;
    public Entity foodPrefab;
}