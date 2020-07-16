using Unity.Entities;
using Unity.Mathematics;

namespace FireBrigade.Components
{
    public struct BucketTarget : IComponentData
    {
        public float3 Position;
        public Entity entity;
    }
}