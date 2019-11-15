using Unity.Entities;

namespace AntPheromones_ECS
{
    public struct Movement : IComponentData
    {
        public float FacingAngle;
        public float Speed;
    }
}