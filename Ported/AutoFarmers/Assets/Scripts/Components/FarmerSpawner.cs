using Unity.Entities;

[GenerateAuthoringComponent]
public struct FarmerSpawner : IComponentData
{
    public Entity FarmerPrefab;
    public int FarmerCounter;
}