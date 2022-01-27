using Unity.Entities;

[InternalBufferCapacity(2)]
public struct RoadNeighbors : IBufferElementData
{
    // These implicit conversions are optional, but can help reduce typing.
    public static implicit operator Entity(RoadNeighbors e) { return e.Value; }
    public static implicit operator RoadNeighbors(Entity e) { return new RoadNeighbors { Value = e }; }

    // Actual value each buffer element will store.
    public Entity Value;
}