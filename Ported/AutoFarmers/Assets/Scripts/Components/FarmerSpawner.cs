using Unity.Entities;

[GenerateAuthoringComponent]
public struct FarmerSpawner : IComponentData
{
    public Entity FarmerPrefab;
    public Entity DronePrefab;
    public int FarmerCounter;
    public int DroneCounter;
}