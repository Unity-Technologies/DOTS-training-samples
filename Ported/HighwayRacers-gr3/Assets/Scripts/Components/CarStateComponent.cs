using Unity.Entities;

public struct CarStateComponent : IComponentData
{
    public CarStates State;
}

public enum CarStates
{
    Cruising,
    LookingToChangeLane,
    Overtaking
}