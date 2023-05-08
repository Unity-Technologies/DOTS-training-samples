using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct DirectionComponent : IComponentData
    {
        public float3 Direction;
    }
}
