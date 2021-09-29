using Unity.Entities;

public struct FoodSpawner : IComponentData
{
    public int InitialFoodCount;
    public bool DidSpawn;
}