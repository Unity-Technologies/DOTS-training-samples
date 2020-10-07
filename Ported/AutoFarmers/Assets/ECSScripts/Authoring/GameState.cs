using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct GameState : IComponentData
{
    public int2 GridSize;
    public Entity PlainsPrefab;
    public Entity TilledPrefab;
    public Entity WaterPrefab;
    public Entity DepotPrefab;
    public Entity ForestPrefab;
    public Entity PlantPrefab;
    public Entity FarmerPrefab;
    public float WaterProbability;
    public float DepotProbability;
    public float ForestProbability;
    public float SimulationSpeed;
}
