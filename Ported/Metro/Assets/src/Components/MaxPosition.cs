using Unity.Entities;

public struct MaxPosition : IComponentData
{
    public float Value;
    
    public static implicit operator float(MaxPosition v) => v.Value;
    public static implicit operator MaxPosition(float v) => new MaxPosition { Value = v };
}
