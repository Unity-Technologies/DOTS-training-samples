using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(16384)]
public struct PheromoneMap : IBufferElementData
{
    public byte Value;
}