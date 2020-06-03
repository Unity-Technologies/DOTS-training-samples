using Unity.Entities;

[GenerateAuthoringComponent]
public struct TargetNextChain : IComponentData
{
    public Entity Target;
}