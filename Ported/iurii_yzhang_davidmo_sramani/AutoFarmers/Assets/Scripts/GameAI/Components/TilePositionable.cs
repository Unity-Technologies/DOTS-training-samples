using Unity.Entities;
using Unity.Mathematics;

namespace GameAI
{
    public struct TilePositionable : IComponentData
    {
        public int2 Position;
    }
}