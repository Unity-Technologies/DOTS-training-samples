using Unity.Entities;

public struct RailPointDistance : IBufferElementData
{
    public float Value;
    
    public static implicit operator float(RailPointDistance v) => v.Value;
    public static implicit operator RailPointDistance(float v) => new RailPointDistance { Value = v };
}
