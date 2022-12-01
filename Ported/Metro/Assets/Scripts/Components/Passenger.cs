using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Apple;

public enum PassengerState
{
    Idle,
    WalkingToPlatform,
    ChoosingQueue,
    WalkingToQueue,
    InQueue,
    OnBoarding,
    OnTrain,
    OffBoarding
}

public struct Passenger : IComponentData
{
    public PassengerState State;
    //public float3 Destination;
}