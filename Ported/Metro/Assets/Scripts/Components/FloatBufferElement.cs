using Unity.Entities;

[InternalBufferCapacity(8)]
public struct FloatBufferElement : IBufferElementData
{
    public float Value;

    public static implicit operator float(FloatBufferElement e) {
        return e.Value;
    }

    public static implicit operator FloatBufferElement(float e) {
        return new FloatBufferElement { Value = e };
    }
}