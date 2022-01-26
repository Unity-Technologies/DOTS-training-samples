using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(64)]
public struct TeamWorkers : IBufferElementData
{
    // These implicit conversions are optional, but can help reduce typing.
    public static implicit operator Entity(TeamWorkers e) { return e.Value; }
    public static implicit operator TeamWorkers(Entity e) { return new TeamWorkers { Value = e }; }

    // Actual value each buffer element will store.
    public Entity Value;
}