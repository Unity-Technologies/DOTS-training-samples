using Unity.Entities;
using Unity.Mathematics;

namespace DOTSRATS
{
    public struct Velocity : IComponentData
    {
        public float3 velocity;
    }
}
