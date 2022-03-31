using Unity.Entities;

public struct CarriageComponent : IComponentData
{
    public Entity Train;
    public int Index;
}