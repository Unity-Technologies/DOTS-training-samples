using Aspects;
using Unity.Entities;

namespace Components
{
    public struct CarDynamicBuffer : IBufferElementData
    {
        public Entity value;
    }
}
