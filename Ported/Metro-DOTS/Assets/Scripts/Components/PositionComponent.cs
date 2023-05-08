using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct PositionComponent : IComponentData
    {
        public float3 Position;
    }
}
