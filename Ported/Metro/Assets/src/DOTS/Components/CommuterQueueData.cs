using Unity.Entities;
using Unity.Mathematics;

namespace src.DOTS.Components
{
    public struct CommuterQueueData : IBufferElementData
    {
        public Entity entity;
    }
}