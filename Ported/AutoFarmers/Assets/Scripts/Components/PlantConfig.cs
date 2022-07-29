using Unity.Entities;
using Unity.Mathematics;

struct PlantConfig : IComponentData
{
    public Entity PlantPrefab;
    public int NumberPlants;
    // public float3 Color;
    // public float RandomScaling;
}