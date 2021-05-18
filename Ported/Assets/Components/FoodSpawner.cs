using Unity.Entities;

[GenerateAuthoringComponent]
public struct FoodSpawner : IComponentData
{
    public Entity FoodPrefab;
}