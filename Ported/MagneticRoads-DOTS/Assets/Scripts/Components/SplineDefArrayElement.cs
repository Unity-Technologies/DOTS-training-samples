using Unity.Entities;

[InternalBufferCapacity(8)]
public struct SplineDefArrayElement : IBufferElementData
{
    // These implicit conversions are optional, but can help reduce typing.
    public static implicit operator SplineDef(SplineDefArrayElement e) { return e.Value; }
    public static implicit operator SplineDefArrayElement(SplineDef e) { return new SplineDefArrayElement { Value = e }; }

    // Actual value each buffer element will store.
    public SplineDef Value;
}