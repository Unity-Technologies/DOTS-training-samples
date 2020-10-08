using Unity.Entities;

public struct RailLength : IComponentData
{
    public float Value;
    
    public static implicit operator float(RailLength v) => v.Value;
    public static implicit operator RailLength(float v) => new RailLength { Value = v };
}
