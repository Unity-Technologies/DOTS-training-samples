using Unity.Entities;

[InternalBufferCapacity(8)]
public struct CarQueue : IBufferElementData
{
    // These implicit conversions are optional, but can help reduce typing.
    public static implicit operator Entity(CarQueue e) { return e.Value; }
    public static implicit operator CarQueue(Entity e) { return new CarQueue { Value = e }; }

    // Actual value each buffer element will store.
    public Entity Value;//TODO change for translation component?? does it even work?
}