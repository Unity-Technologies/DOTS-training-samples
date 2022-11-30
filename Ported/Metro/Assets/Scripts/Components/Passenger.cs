using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Apple;

public enum PassengerState
{
    Walking,
    Waiting,
    InQueue,
    OnBoard
}

public struct Passenger : IComponentData
{
    public PassengerState State;
    public float3 Destination;
}