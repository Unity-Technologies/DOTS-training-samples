using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public enum PassengerState
{
    Walk,
    GetOnTheTrain,
    GetOffTheTrain,
    Queue
}

public struct Passenger : IComponentData
{
    public PassengerState State;
    public NativeArray<float3> Pathway;
}