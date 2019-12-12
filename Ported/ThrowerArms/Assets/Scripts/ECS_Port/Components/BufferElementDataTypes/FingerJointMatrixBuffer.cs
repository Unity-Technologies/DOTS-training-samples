using Unity.Entities;
using UnityEngine;

public struct FingerJointMatrixBuffer : IBufferElementData
{
    public Matrix4x4 Value;
    
    public static implicit operator FingerJointMatrixBuffer(Matrix4x4 matrix) =>
        new FingerJointMatrixBuffer {Value = matrix};

    public static implicit operator Matrix4x4(FingerJointMatrixBuffer buffer) => buffer.Value; 
}