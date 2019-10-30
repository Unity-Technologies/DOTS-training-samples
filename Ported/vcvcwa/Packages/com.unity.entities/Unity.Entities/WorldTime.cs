using Unity.Core;

namespace Unity.Entities
{
    internal struct WorldTime : IComponentData
    {
        public TimeData Time;
    }

    internal struct WorldTimeQueue : IBufferElementData
    {
        public TimeData Time;
    }
}