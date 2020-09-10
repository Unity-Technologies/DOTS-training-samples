using Unity.Entities;

[GenerateAuthoringComponent]
public struct Arrow : IComponentData
{
    public Entity Owner;
}
