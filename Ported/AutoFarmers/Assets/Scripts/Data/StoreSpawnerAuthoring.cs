using Unity.Entities;
using Unity.Mathematics;
namespace AutoFarmers
{
    [GenerateAuthoringComponent]
    public struct StoreSpawner : IComponentData
    {
        public Entity StorePrefab;
        public Entity FarmerPrefab;
        public Entity DronePrefab;
        public int NumStores;
        public int MinDistance;
        public int DroneCost;
        public int FarmerCost;
    }
}