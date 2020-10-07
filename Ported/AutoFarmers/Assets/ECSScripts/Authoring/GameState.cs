using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct GameState : IComponentData
{
    public int2 GridSize;
    public Entity PlainsPrefab;
    public Entity WaterPrefab;
    public Entity PlantPrefab;
    public float WaterProbability;
<<<<<<< HEAD
    public float SimulationSpeed;
=======
    
    public Entity FarmerPrefab;
>>>>>>> ca02060284fc3a0fbb1fea2da6b34dfc55d99388
}
