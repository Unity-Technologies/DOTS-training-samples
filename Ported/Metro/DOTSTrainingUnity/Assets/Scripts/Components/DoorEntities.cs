using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(8)]
public struct DoorEntities : IBufferElementData
{
    public Entity value;
	
	// The following implicit conversions are optional, but can be convenient.
    public static implicit operator int(DoorEntities e)
    {
        return e.Value;
    }

    public static implicit operator DoorEntities(int e)
    {
        return new DoorEntities { Value = e };
    }
}
