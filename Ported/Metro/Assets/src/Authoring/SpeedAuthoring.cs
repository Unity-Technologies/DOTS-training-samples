using Unity.Entities;

[GenerateAuthoringComponent]
public struct Speed : IComponentData
{
    public float Value;
    
    public static implicit operator float(Speed v) => v.Value;
    public static implicit operator Speed(float v) => new Speed { Value = v };
}
