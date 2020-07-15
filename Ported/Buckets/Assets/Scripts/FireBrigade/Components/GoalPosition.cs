using Unity.Entities;
using Unity.Mathematics;

namespace FireBrigade.Components
{
    public struct GoalPosition : IComponentData
    {
        public float3 Value;
    }
}