using Unity.Entities;

public struct TrainStateComponent : IComponentData
{
    public TrainNavigationSystem.TrainState Value;
}