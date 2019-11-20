using Unity.Entities;

namespace AntPheromones_ECS
{
    public struct PheromoneSteeringComponent : IComponentData
    {
        public float Value;
    }
}