using Unity.Entities;

[GenerateAuthoringComponent]
public struct TargetedEntity : IComponentData
{
    public Entity Value;
}