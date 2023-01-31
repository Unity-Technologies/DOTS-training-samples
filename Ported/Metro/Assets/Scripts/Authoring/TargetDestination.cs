using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct TargetDestination : IComponentData
{
    public bool IsActive;
    public float3 TargetPosition;
}

