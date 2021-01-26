using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct InitializationSpawner : IComponentData
{
    public int NumberOfBees;
    public int NumberOfFood;
    
    public AABB FoodSpawnBox;
}
