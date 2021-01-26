using Unity.Entities;
using Unity.Mathematics;

public struct InitializationSpawner : IComponentData
{
    public Entity BeePrefab;
    public int NumberOfBees;
    
    public Entity FoodPrefab;
    public int NumberOfFood;
    public AABB FoodSpawnBox;
}
