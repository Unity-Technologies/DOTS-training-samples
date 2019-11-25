using Unity.Entities;

namespace AntPheromones_ECS
{
    public struct ResourceCarrier : IComponentData
    {
        public bool IsCarrying;
    }
}