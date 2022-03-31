using Unity.Entities;

// TODO : Think more about it
[InternalBufferCapacity(0)]
public struct HeightElement : IBufferElementData
{
    public float Value;

    public static implicit operator float(HeightElement e)
    {
        return e.Value;
    }

    public static implicit operator HeightElement(float value)
    {
        return new HeightElement { Value = value };
    }
}
