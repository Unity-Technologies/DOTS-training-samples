using Unity.Entities;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    public struct VelocityComponent : IComponentData
    {
        public float2 Value;
    }
}