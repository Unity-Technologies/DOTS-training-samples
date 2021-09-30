using Unity.Entities;
using Unity.Mathematics;

public struct FoodSpawner : IComponentData
{
    public int InitialFoodCount;
    public bool DidSpawn;
    public float3 SpawnLocation;
    public int ResourceCount;
    public bool PlaceFood;
}