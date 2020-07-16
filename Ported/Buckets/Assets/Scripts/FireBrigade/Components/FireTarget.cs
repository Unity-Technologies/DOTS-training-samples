using Unity.Entities;
using Unity.Mathematics;

namespace FireBrigade.Components
{
    public struct FireTarget : IComponentData
    {
        public float3 Position;
        public Entity entity;
    }
}