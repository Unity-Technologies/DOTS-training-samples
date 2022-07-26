using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct CarBootstrap : IComponentData
    {
        public int CarsToInitialise;
        public Entity CarPrefab;
    }
}
