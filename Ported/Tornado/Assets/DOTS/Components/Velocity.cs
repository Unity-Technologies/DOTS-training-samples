using Unity.Entities;
using Unity.Mathematics;

namespace Dots
{
    public struct Velocity : IComponentData
    {
        public float3 value;
    }
}