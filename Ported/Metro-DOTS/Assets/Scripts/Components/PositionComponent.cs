using Unity.Entities;
using Unity.Mathematics;

namespace Metro
{
    public struct PositionComponent : IComponentData
    {
        public float3 Position;
    }
}
