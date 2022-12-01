using Unity.Entities;

public enum TrainState
{
    EnRoute,
    Arriving,
    Unloading,
    WaitingForUnloading,
    Arrived,
    WaitingForLoading,
    WaitingOnPlatform,
    Moving,
    Loading,
    Departing
}

public struct TrainStateComponent : IComponentData
{
    public TrainState State;
}