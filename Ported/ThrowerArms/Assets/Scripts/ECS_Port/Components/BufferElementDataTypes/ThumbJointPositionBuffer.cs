using Unity.Entities;
using Unity.Mathematics;

public struct ThumbJointPositionBuffer : IBufferElementData
{
    public float3 Value;
	
    public static implicit operator ThumbJointPositionBuffer(float3 matrix) =>
        new ThumbJointPositionBuffer {Value = matrix};

    public static implicit operator float3(ThumbJointPositionBuffer buffer) => buffer.Value; 
}