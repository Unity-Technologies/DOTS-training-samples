using Unity.Entities;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    public struct Velocity : IComponentData
    {
        public float2 Value;
    }
}