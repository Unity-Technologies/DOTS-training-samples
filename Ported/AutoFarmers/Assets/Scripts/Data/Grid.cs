using Unity.Entities;
using Unity.Mathematics;

namespace AutoFarmers
{
    // Singleton
    struct Grid : IComponentData
    {
        public int2 Size;
        public int GetIndexFromCoords(int x, int y)
        {
            return x * Size.x + y;
        }

        public int GetIndexFromCoords(int2 p)
        {
            return GetIndexFromCoords(p.x, p.y);
        }
    }
}