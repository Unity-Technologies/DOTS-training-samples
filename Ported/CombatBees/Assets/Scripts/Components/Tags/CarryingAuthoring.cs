using Unity.Entities;

[GenerateAuthoringComponent]
public struct Carrying : IComponentData
{
    public Entity Value;
}