using Unity.Entities;
using Unity.Mathematics;
namespace AutoFarmers
{
    [GenerateAuthoringComponent]
    public struct PlantSpawner : IComponentData
    {
        // We use this to get access to a plant prefab
        public Entity PlantPrefab;
        public float GrowRate;
    }
}