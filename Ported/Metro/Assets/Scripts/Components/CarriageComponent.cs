using Unity.Entities;

[GenerateAuthoringComponent]
public struct CarriageComponent : IComponentData
{
    public Entity Train;
    public int Index;
}