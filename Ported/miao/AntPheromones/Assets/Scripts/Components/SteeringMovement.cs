using Unity.Entities;

namespace AntPheromones_ECS
{
    public struct SteeringMovement : IComponentData
    {
        public float MaxSpeed;
        public float Acceleration;
    }
}