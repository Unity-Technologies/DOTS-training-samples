using Unity.Entities;
using Unity.Mathematics;

public struct BeeMovement : IComponentData
{
    public float3 Velocity;
    public float Speed;
}