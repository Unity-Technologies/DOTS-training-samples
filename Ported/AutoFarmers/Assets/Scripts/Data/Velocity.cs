using Unity.Entities;

[GenerateAuthoringComponent]
public struct Velocity : IComponentData
{
    public Entity Value;
}