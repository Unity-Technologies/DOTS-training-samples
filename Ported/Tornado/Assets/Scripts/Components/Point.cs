using Unity.Entities;
using Unity.Mathematics;


// This describes the number of buffer elements that should be reserved
// in chunk data for each instance of a buffer. In this case, 8 integers
// will be reserved (32 bytes) along with the size of the buffer header
// (currently 16 bytes on 64-bit targets)
//[InternalBufferCapacity(8)]
//TODO: ask fabrice, could we have just a buffer of floats? Do we need a struct? Does burst optmizes this somehow?
public struct CurrentPoint : IBufferElementData
{
    public float3 Value;
}

public struct PreviousPoint : IBufferElementData
{
    public float3 Value;
}

public struct AnchorPoint : IBufferElementData
{
    public bool Value;
}

public struct NeighborCount : IBufferElementData
{
    public int Value;
}

