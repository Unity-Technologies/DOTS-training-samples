using Unity.Entities;
using UnityEngine;

public struct ArmJointMatrixBuffer : IBufferElementData
{
    public Matrix4x4 Value;
}