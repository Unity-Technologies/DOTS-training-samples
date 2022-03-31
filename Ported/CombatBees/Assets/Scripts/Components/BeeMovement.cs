using Unity.Entities;
using Unity.Mathematics;

public struct BeeMovement : IComponentData
{
    public byte IsAttacking;
}

public struct MovementSmoothing : IComponentData
{
    public float Size;
    public float3 SmoothPosition;
    public float3 SmoothDirection;
}

