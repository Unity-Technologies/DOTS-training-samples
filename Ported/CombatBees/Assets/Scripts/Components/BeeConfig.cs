using Unity.Entities;

namespace Components
{
    public struct BeeConfig : IComponentData
    {
        public Entity BeePrefab;
        public int BeesToSpawn;
        public float MinBeeSize;
        public float MaxBeeSize;
    }
}
