using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct CarriageSeats : IComponentData
{
    public NativeArray<float3> Seats;
    public DynamicBuffer<Entity> Passengers;
}