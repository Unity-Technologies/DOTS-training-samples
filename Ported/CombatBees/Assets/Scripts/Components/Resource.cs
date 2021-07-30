using Unity.Entities;

[GenerateAuthoringComponent]
public struct Resource : IComponentData
{
    public float Speed;
    public Entity CarryingBee;
}
