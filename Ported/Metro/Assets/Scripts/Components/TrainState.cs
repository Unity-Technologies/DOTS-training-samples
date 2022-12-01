using Unity.Entities;

public enum TrainState
{
    EnRoute,
    Arriving,
    Unloading,
    Arrived,
    WaitingOnPlatform,
    Moving,
    Loading,
    Departing
}

public struct TrainStateComponent : IComponentData
{
    public TrainState State;
}