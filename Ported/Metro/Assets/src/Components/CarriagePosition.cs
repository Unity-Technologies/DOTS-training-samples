using Unity.Entities;

public struct CarriagePosition : IComponentData
{
    public float Value;
    
    public static implicit operator float(CarriagePosition v) => v.Value;
    public static implicit operator CarriagePosition(float v) => new CarriagePosition { Value = v };
}
