using Unity.Entities;
using Unity.Mathematics;

namespace AutoFarmers
{
    // Singleton
    struct Grid : IComponentData
    {
        public int2 Size;
    }
}