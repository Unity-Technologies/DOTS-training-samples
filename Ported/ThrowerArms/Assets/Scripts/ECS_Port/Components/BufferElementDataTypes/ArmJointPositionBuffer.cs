using Unity.Entities;
using Unity.Mathematics;

public struct ArmJointPositionBuffer : IBufferElementData
{
    public float3 Value;
	
    public static implicit operator ArmJointPositionBuffer(float3 matrix) =>
        new ArmJointPositionBuffer {Value = matrix};

    public static implicit operator float3(ArmJointPositionBuffer buffer) => buffer.Value; 
}