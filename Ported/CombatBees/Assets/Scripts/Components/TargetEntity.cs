using Unity.Entities;
using Unity.Mathematics;

public struct TargetEntity : IComponentData
{
    public Entity Value;
    public float3 Position;
}
