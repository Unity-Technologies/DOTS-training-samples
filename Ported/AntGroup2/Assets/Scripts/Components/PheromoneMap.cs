using Unity.Entities;

[InternalBufferCapacity(1)] // Can this be 0?
struct PheromoneMap : IBufferElementData
{
    public float amount;
}