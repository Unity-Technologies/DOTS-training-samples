using Unity.Entities;
using UnityEngine;

public struct ThumbJointMatrixBuffer : IBufferElementData
{
    public Matrix4x4 Value;
    
    public static implicit operator ThumbJointMatrixBuffer(Matrix4x4 matrix) =>
        new ThumbJointMatrixBuffer {Value = matrix};

    public static implicit operator Matrix4x4(ThumbJointMatrixBuffer buffer) => buffer.Value; 
}