using Unity.Entities;

namespace AntPheromones_ECS
{
    public struct SteeringMovementComponent : IComponentData
    {
        public float MaxSpeed;
        public float Acceleration;
    }
}