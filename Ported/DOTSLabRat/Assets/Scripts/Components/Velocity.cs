using Unity.Entities;
using Unity.Mathematics;

namespace DOTSRATS
{
    [GenerateAuthoringComponent]
    public struct Velocity : IComponentData
    {
        public float3 velocity;
    }
}
