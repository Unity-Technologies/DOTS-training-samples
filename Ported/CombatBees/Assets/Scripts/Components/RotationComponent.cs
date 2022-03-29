using System.Numerics;
using Unity.Entities;

public struct RotationComponent : IComponentData
{
    public Matrix4x4 Value;
}
