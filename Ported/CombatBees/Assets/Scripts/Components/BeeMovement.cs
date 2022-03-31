using Unity.Entities;
using Unity.Mathematics;

public struct BeeMovement : IComponentData
{
    public float3 Velocity;
    public float Size;
    public byte IsAttacking;
}

public struct MovementSmoothing : IComponentData
{
    public float3 SmoothPosition;
    public float3 SmoothDirection;
}

