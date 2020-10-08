using Unity.Entities;
using Unity.Mathematics;

public struct RailPoint : IBufferElementData
{
    public float3 Value;
    
    public static implicit operator float3(RailPoint v) => v.Value;
    public static implicit operator RailPoint(float3 v) => new RailPoint { Value = v };
}
