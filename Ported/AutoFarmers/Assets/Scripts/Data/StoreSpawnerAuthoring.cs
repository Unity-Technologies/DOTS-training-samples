using Unity.Entities;
using Unity.Mathematics;
namespace AutoFarmers
{
    [GenerateAuthoringComponent]
    public struct StoreSpawner : IComponentData
    {
        public Entity StorePrefab;
        public int NumStores;
        public int MinDistance;
    }
}