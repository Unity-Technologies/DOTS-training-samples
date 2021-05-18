using Unity.Entities;
using Unity.Mathematics;

public struct Target : IComponentData
{
    public Entity Value;
}
public struct TargetPosition : IComponentData
{
    public float3 Value;
}
