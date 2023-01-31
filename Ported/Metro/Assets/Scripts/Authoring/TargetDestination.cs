using Unity.Entities;
using Unity.Mathematics;

struct TargetDestination : IComponentData
{
    public bool IsActive;
    public float3 TargetPosition;
}

