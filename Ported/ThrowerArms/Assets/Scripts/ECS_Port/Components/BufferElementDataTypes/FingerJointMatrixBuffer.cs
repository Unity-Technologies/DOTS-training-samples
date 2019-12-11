using Unity.Entities;
using UnityEngine;

public struct FingerJointMatrixBuffer : IBufferElementData
{
    public Matrix4x4 Value;
}