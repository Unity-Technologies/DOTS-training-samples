using Unity.Entities;

namespace src.DOTS.Components
{
    public struct QueueingToEmbarkTag : IComponentData
    {
        public int platform;
    }
}