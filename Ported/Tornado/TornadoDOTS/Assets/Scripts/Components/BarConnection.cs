using Unity.Entities;

public struct BarConnection : IComponentData
{
    public Entity Joint1;
    public Entity Joint2;
    public float Length;
}