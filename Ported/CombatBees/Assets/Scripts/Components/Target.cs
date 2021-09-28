using Unity.Entities;

[GenerateAuthoringComponent]
public struct Target : IComponentData
{
    public Entity Value;
}

