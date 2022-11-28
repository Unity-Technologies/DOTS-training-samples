using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct Carriage : IComponentData
{
    public NativeArray<float2> Seats;
    public NativeList<Entity> Passengers;
}