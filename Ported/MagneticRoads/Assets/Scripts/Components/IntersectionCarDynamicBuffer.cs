using Aspects;
using Unity.Entities;

namespace Components
{
    public struct IntersectionCarDynamicBuffer : IBufferElementData
    {
        public Entity value;
    }
}
