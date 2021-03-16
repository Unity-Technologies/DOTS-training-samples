using Unity.Entities;

namespace src.DOTS.Components
{
    public struct ConnectedPlatforms : IComponentData
    {
        public Entity front;
        public Entity back;
    }
}