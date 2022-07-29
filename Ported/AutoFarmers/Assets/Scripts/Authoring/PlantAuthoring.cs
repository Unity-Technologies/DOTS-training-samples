using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class PlantAuthoring : MonoBehaviour
{
    public GameObject PlantPrefab;
    public int NumberPlants;
    // public float3 Color;
    // public float RandomScaling;
}
class PlantBaker : Baker<PlantAuthoring>
{
    public override void Bake(PlantAuthoring authoring)
    {
        AddComponent(new PlantConfig()
        {
            PlantPrefab =GetEntity(authoring.PlantPrefab),
            NumberPlants = authoring.NumberPlants,
            // Color = authoring.Color,
            // RandomScaling = authoring.RandomScaling,

        });
    }
}