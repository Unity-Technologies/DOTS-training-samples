using Unity.Entities;
using Unity.Mathematics;

public struct UpVectorBufferForArmsAndFingers : IBufferElementData
{
    public float3 Value;
    
    public static implicit operator UpVectorBufferForArmsAndFingers(float3 matrix) =>
        new UpVectorBufferForArmsAndFingers {Value = matrix};

    public static implicit operator float3(UpVectorBufferForArmsAndFingers buffer) => buffer.Value; 
}