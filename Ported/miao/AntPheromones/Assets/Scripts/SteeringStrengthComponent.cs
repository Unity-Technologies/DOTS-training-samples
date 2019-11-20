using Unity.Entities;

namespace AntPheromones_ECS
{
    public struct SteeringStrengthComponent : IComponentData
    {
        public float Random;
        public float Wall;
        public float Pheromone;
        public float Target;
        public float Inward;
        public float Outward;
    }

    public struct SteeringMovementComponent : IComponentData
    {
        public float MaxSpeed;
        public float Acceleration;
    }
}