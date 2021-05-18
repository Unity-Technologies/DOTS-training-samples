using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(1024)]
public struct Pheromone : IBufferElementData
{
    public byte Value;
}