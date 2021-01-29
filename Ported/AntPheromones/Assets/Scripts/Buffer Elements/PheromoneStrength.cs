using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

// This describes the number of buffer elements that should be reserved
// in chunk data for each instance of a buffer. In this case, 8 integers
// will be reserved (32 bytes) along with the size of the buffer header
// (currently 16 bytes on 64-bit targets)

[InternalBufferCapacity(8)]
public struct PheromoneStrength : IBufferElementData
{
    public static implicit operator byte(PheromoneStrength e) { return e.Value; }
    public static implicit operator PheromoneStrength(byte e) { return new PheromoneStrength { Value = e }; }
    public byte Value;
}