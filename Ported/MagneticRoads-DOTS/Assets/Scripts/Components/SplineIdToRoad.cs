using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(8)]
public struct SplineIdToRoad : IBufferElementData
{
    // These implicit conversions are optional, but can help reduce typing.
    public static implicit operator Entity(SplineIdToRoad e) { return e.Value; }
    public static implicit operator SplineIdToRoad(Entity e) { return new SplineIdToRoad { Value = e }; }

    // Actual value each buffer element will store.
    public Entity Value;
}