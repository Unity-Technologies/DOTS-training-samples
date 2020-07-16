using Unity.Entities;
using Unity.Mathematics;

namespace FireBrigade.Components
{
    public struct WaterTarget : IComponentData
    {
        public float3 Position;
        public Entity entity;
    }
}