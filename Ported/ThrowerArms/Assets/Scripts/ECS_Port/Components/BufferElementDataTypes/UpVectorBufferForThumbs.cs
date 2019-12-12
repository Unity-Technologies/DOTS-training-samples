using Unity.Entities;
using Unity.Mathematics;

public struct UpVectorBufferForThumbs : IBufferElementData
{
    public float3 Value;
    
    public static implicit operator UpVectorBufferForThumbs(float3 matrix) =>
        new UpVectorBufferForThumbs {Value = matrix};

    public static implicit operator float3(UpVectorBufferForThumbs buffer) => buffer.Value; 
}