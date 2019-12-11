using Unity.Entities;
using UnityEngine;

public struct BoneMatrixBuffer : IBufferElementData
{
    public Matrix4x4 Value;

    public static implicit operator BoneMatrixBuffer(Matrix4x4 matrix) => 
        new BoneMatrixBuffer {Value = matrix};

    public static implicit operator Matrix4x4(BoneMatrixBuffer matrixBuffer) => matrixBuffer.Value; 
}