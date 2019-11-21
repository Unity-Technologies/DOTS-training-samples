using Unity.Entities;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    public struct LocalToWorldComponent : IComponentData
    {
        public float4x4 Value;
    }
}