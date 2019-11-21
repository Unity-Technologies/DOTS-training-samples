using Unity.Entities;

namespace AntPheromones_ECS
{
    public struct ResourceCarrierComponent : IComponentData
    {
        public bool IsCarrying;
    }
}