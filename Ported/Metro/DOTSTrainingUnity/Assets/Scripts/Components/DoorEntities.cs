using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(8)]
public struct DoorEntities : IBufferElementData
{
    public Entity value;
	
	// The following implicit conversions are optional, but can be convenient.
    public static implicit operator Entity(DoorEntities e)
    {
        return e.value;
    }

    public static implicit operator DoorEntities(Entity e)
    {
        return new DoorEntities { value = e };
    }
}
