using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(8)]
public struct SplineLink : IBufferElementData
{
    // These implicit conversions are optional, but can help reduce typing.
    public static implicit operator int2(SplineLink e) { return e.Value; }
    public static implicit operator SplineLink(int2 e) { return new SplineLink { Value = e }; }

    // Actual value each buffer element will store.
    public int2 Value;
}