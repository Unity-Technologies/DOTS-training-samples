using Unity.Entities;

public struct FoodSpawner : IComponentData
{
    public Entity FoodPrefab;
    public int InitialFoodCount;

}