using Unity.Entities;

[GenerateAuthoringComponent]
public struct ExecutingCommand : IComponentData
{
    public bool Value;
}