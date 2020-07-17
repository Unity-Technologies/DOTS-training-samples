using Unity.Entities;
using Unity.Mathematics;

namespace Water
{
    public struct Attached : IComponentData
    {
        public Entity Value;
        public float3 Offset;
    }
}