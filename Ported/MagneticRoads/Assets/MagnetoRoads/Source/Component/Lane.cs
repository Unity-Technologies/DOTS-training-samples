using Unity.Entities;

[InternalBufferCapacity(8)]
public struct MyBufferElement : IBufferElementData
{
    // Actual value each buffer element will store.
    public Entity Value;

    // The following implicit conversions are optional, but can be convenient.
    public static implicit operator Entity(MyBufferElement e)
    {
        return e.Value;
    }

    public static implicit operator MyBufferElement(Entity e)
    {
        return new MyBufferElement { Value = e };
    }
}

[GenerateAuthoringComponent]
public struct Lane : IComponentData
{
    public float Length;
}
