using Unity.Entities;

// This describes the number of buffer elements that should be reserved
// in chunk data for each instance of a buffer. In this case, 8 bools
// will be reserved (32 bytes) along with the size of the buffer header
// (currently 16 bytes on 64-bit targets)
[InternalBufferCapacity(256)]
public struct OccupiedBufferElement : IBufferElementData
{
    // These implicit conversions are optional, but can help reduce typing.
    public static implicit operator bool(OccupiedBufferElement e) { return e.Value; }
    public static implicit operator OccupiedBufferElement(bool e) { return new OccupiedBufferElement { Value = e }; }

    // Actual value each buffer element will store.
    public bool Value;
}