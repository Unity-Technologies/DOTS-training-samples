using Unity.Entities;

[InternalBufferCapacity(0)]
public struct Pheromone: IBufferElementData
{
    public float Value;
}
