using Unity.Entities;
using Unity.Mathematics;

public struct Physical : IComponentData
{
    public enum FieldCollisionType
    {
        Bounce,             // reflects velocity, 100% energy conversion
        Splat,              // zeroes velocity, and stops falling
        Slump,              // zeroes velocity, but can continue to fall
    }

    public float3 Position;
    public float3 Velocity;
    public bool IsFalling;
    public FieldCollisionType Collision;
}