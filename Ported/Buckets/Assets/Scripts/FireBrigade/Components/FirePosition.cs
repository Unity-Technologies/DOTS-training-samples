using Unity.Entities;
using Unity.Mathematics;

namespace FireBrigade.Components
{
    public struct FirePosition : IComponentData
    {
        public float3 Value;
    }
}