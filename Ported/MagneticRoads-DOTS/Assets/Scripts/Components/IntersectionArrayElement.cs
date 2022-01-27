using Unity.Entities;

[InternalBufferCapacity(8)]
public struct IntersectionArrayElement : IBufferElementData
{
    // These implicit conversions are optional, but can help reduce typing.
    public static implicit operator Entity(IntersectionArrayElement e) { return e.Value; }
    public static implicit operator IntersectionArrayElement(Entity e) { return new IntersectionArrayElement { Value = e }; }

    // Actual value each buffer element will store.
    public Entity Value;
}
