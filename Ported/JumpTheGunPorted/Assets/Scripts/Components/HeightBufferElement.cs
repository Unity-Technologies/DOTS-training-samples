using Unity.Entities;

// This describes the number of buffer elements that should be reserved
// in chunk data for each instance of a buffer. In this case, 256 floats
// will be reserved (256 x 4 bytes) along with the size of the buffer header
// (currently 16 bytes on 64-bit targets)
[InternalBufferCapacity(256)]
public struct HeightBufferElement : IBufferElementData
{
    // These implicit conversions are optional, but can help reduce typing.
    public static implicit operator float(HeightBufferElement e) { return e.Value; }
    public static implicit operator HeightBufferElement(float e) { return new HeightBufferElement { Value = e }; }

    // Actual value each buffer element will store.
    public float Value;
}