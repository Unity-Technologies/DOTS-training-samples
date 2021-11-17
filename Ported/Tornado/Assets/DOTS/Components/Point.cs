using Unity.Entities;
using Unity.Mathematics;

namespace Dots
{
    public struct Point : IComponentData
    {
        public float3 value;
        public float3 old;
    }
}

