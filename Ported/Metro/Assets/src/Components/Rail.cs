using Unity.Entities;

public struct Rail : IComponentData
{
    public Entity Value;
    
    public static implicit operator Entity(Rail v) => v.Value;
    public static implicit operator Rail(Entity v) => new Rail { Value = v };
}
