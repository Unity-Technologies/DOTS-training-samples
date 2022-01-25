using Unity.Entities;

[GenerateAuthoringComponent]
public struct CarriedEntity : IComponentData
{
    public Entity Value;
}