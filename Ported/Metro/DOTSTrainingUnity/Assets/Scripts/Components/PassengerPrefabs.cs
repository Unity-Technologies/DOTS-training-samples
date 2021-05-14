using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(5)]
public struct PassengerPrefabs : IBufferElementData
{
    public Entity value;
	
	// The following implicit conversions are optional, but can be convenient.
    public static implicit operator Entity(PassengerPrefabs e)
    {
        return e.value;
    }

    public static implicit operator PassengerPrefabs(Entity e)
    {
        return new PassengerPrefabs { value = e };
    }
}
