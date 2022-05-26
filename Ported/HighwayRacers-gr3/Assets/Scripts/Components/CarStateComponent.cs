using Unity.Entities;

[GenerateAuthoringComponent]
public struct CarStateComponent : IComponentData
{
    public CarStates State;
    public int StateAsInt;
}

public enum CarStates
{
    Cruising,
    LookingToChangeLane,
    Overtaking
}