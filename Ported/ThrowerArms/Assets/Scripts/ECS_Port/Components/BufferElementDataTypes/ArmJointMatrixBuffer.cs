using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct ArmJointMatrixBuffer : IBufferElementData
{
    public Matrix4x4 Value;
 
    public static implicit operator ArmJointMatrixBuffer(Matrix4x4 matrix) =>
        new ArmJointMatrixBuffer {Value = matrix};

    public static implicit operator Matrix4x4(ArmJointMatrixBuffer buffer) => buffer.Value; 
}

public struct ArmBoneBuffer : IBufferElementData
{
    public int StartIndex;
    public int EndIndex;
    public float Thickness;
    public int upIndex;
}

public struct ArmUpVectorBuffer : IBufferElementData
{
    public float3 up;
}

public struct ArmJointBuffer : IBufferElementData
{
    public float3 pos;
}