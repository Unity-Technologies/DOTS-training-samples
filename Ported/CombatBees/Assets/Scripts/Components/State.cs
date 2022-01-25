using Unity.Entities;

public enum StateValues
{
    Attacking,
    Carrying,
    Seeking
}


[GenerateAuthoringComponent]
public struct State : IComponentData
{
    public StateValues value;
}