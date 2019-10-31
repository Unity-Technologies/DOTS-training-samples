using Unity.Entities;

[GenerateAuthoringComponent]
public struct CollectedEntity : IComponentData
{
    public Entity Value;
}