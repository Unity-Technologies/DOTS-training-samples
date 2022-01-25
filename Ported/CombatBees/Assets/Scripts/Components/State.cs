using Unity.Entities;

[GenerateAuthoringComponent]
public struct State : IComponentData
{
    public int value;
}