using Unity.Entities;
using Unity.Mathematics;

namespace FireBrigade.Components
{
    public struct WaterPosition : IComponentData
    {
        public float3 Value;
    }
}