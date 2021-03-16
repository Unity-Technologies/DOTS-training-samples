using Unity.Entities;
using Unity.Mathematics;

namespace src.DOTS.Components
{
    public struct PathData : IBufferElementData
    {
        public float3 point;
    }
}