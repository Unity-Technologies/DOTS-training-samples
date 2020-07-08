using Unity.Entities;
using Unity.Mathematics;
namespace AutoFarmers
{
    [GenerateAuthoringComponent]
    public struct RockSpawner : IComponentData
    {
        public Entity RockPrefab;
        public int NumRocks;
        public int2 RandomSizeMin;
        public int2 RandomSizeMax;
        public float minHeight;
        public float maxHeight;
    }
}