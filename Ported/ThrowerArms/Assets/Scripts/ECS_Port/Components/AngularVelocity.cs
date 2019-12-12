using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct AngularVelocity : IComponentData
{
    public float3 Value;
}