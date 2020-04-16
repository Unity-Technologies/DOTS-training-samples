using Unity.Entities;

[GenerateAuthoringComponent]
public struct ThrowerTag : IComponentData
{
    public Entity brigade;
}