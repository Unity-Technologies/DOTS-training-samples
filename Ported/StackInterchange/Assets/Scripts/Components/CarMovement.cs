using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct CarMovement : IComponentData
{
    public Entity NextNode;
    public float Velocity;
    public float Acceleration;
    public float Deceleration;
    public float MaxSpeed;
    public float distanceTraveled;
    public float distanceToNext;
    public float3 travelVec;
}
