using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct CarMovement : IComponentData
{
    public Translation NextNode; // TODO: replace with Nodes
    public float Velocity;
    public float Acceleration;
    public float Deceleration;
    public float MaxSpeed;
}