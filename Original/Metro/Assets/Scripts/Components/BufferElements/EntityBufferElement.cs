using Unity.Entities;

namespace dots_src.Components
{
    [InternalBufferCapacity(8)]
    public struct EntityBufferElement : IBufferElementData
    {
        public Entity Value;
        public static implicit operator Entity(EntityBufferElement element) => element.Value;
        public static implicit operator EntityBufferElement(Entity entity) => new EntityBufferElement{Value = entity};
    }
}
