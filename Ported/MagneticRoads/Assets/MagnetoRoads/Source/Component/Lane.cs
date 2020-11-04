using Unity.Entities;

[InternalBufferCapacity(8)]
public struct CarBufferElement : IBufferElementData
{
    // Actual value each buffer element will store.
    public Entity Value;

    // The following implicit conversions are optional, but can be convenient.
    public static implicit operator Entity(CarBufferElement e)
    {
        return e.Value;
    }

    public static implicit operator CarBufferElement(Entity e)
    {
        return new CarBufferElement { Value = e };
    }
}

[GenerateAuthoringComponent]
public struct Lane : IComponentData
{
    public float Length;
}
