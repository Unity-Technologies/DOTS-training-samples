using Unity.Entities;

namespace AntPheromones_ECS
{
    public struct SteeringStrength : IComponentData
    {
        public float Random;
        public float Wall;
        public float Pheromone;
        public float Target;
        public float Inward;
        public float Outward;
    }

    public struct SteeringMovement : IComponentData
    {
        public float MaxSpeed;
        public float Acceleration;
    }
}