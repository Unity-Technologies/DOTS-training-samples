using Unity.Entities;

public enum StateValues
{
    Idle,
    Attacking,
    Carrying,
    Seeking
}


[GenerateAuthoringComponent]
public struct State : IComponentData
{
    public StateValues value;
}