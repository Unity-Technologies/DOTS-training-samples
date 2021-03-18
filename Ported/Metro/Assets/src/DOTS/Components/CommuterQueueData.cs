using Unity.Entities;
using Unity.Mathematics;

namespace src.DOTS.Components
{
    [GenerateAuthoringComponent]
    public struct CommuterQueueData : IBufferElementData
    {
        public Entity entity;
    }
}