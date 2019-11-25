using Unity.Entities;

namespace AntPheromones_ECS
{
    public struct SteeringStrength : IComponentData
    {
        public float Random;
        public float Wall;
        public float Pheromone;
        public float Goal;
        public float Inward;
        public float Outward;
    }
}