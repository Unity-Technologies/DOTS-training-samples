using Unity.Entities;

public struct Position : IComponentData
{
    public float Value;
    
    public static implicit operator float(Position v) => v.Value;
    public static implicit operator Position(float v) => new Position { Value = v };
}
