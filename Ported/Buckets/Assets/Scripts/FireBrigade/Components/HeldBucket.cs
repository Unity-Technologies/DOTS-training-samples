using Unity.Entities;

namespace FireBrigade.Components
{
    public struct HeldBucket : IComponentData
    {
        public Entity Value;
    }
}