using Unity.Entities;
using UnityEngine;

public struct ThumbJointMatrixBuffer : IBufferElementData
{
    public Matrix4x4 Value;
}