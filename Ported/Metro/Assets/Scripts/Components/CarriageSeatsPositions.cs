using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct CarriageSeatsPositions : IComponentData
{
    public NativeArray<float3> Seats;
}

[InternalBufferCapacity(27)]
public struct CarriagePassengers : IBufferElementData
{
    public Entity Value;
}
