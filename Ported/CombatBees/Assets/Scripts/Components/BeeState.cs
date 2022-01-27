using Unity.Entities;

public enum StateValues
{
    Idle,
    Attacking,
    Carrying,
    Seeking
}


[GenerateAuthoringComponent]
public struct BeeState : IComponentData
{
    public StateValues value;
}