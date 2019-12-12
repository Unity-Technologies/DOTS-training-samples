using Unity.Entities;
using UnityEngine;

public struct ArmJointMatrixBuffer : IBufferElementData
{
    public Matrix4x4 Value;
    
    public static implicit operator ArmJointMatrixBuffer(Matrix4x4 matrix) =>
        new ArmJointMatrixBuffer {Value = matrix};

    public static implicit operator Matrix4x4(ArmJointMatrixBuffer buffer) => buffer.Value; 
}