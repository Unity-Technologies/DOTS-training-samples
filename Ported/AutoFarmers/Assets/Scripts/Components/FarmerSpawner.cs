using Unity.Entities;

public struct FarmerSpawner : IComponentData
{
    public Entity FarmerPrefab;
    public int FarmerCounter;
}