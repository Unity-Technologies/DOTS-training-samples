using Unity.Entities;

namespace Components
{
    partial struct TestSplineConfig : IComponentData
    {
        public Entity CarPrefab;
        public int CarCount;
        public int WorldSize;
    }
}