using Unity.Entities;

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
}