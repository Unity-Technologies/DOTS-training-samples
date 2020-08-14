using Unity.Entities;
using Unity.Mathematics;

public struct BucketMovement : IComponentData
{
    public float3 Value; // TargetPosition
}