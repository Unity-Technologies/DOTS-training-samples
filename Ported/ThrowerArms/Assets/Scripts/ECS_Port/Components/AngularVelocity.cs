using Unity.Entities;
using Unity.Transforms;

public struct AngularVelocity : IComponentData
{
    public Rotation rotation;
}