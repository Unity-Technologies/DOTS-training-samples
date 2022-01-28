using Unity.Entities;

public enum StateValues
{
    Idle,
    Attacking,
    Carrying,
    Seeking,
    Wandering,
}


[GenerateAuthoringComponent]
public struct BeeState : IComponentData
{
    public StateValues value;
}