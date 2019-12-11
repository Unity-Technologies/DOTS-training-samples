using Unity.Entities;
using Unity.Mathematics;

public struct FingerJointPositionBuffer : IBufferElementData
{
    public float3 Value;
	
    public static implicit operator FingerJointPositionBuffer(float3 matrix) =>
        new FingerJointPositionBuffer {Value = matrix};

    public static implicit operator float3(FingerJointPositionBuffer buffer) => buffer.Value; 
}