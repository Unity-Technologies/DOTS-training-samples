using Unity.Entities;

// TODO : Think more about it
[InternalBufferCapacity(0)]
public struct OccupiedElement : IBufferElementData
{
    public bool value;

    public static implicit operator bool(OccupiedElement e)
    {
        return e.value;
    }

    public static implicit operator OccupiedElement(bool b)
    {
        return new OccupiedElement {value = b};
    }
}
