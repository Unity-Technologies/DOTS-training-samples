using Unity.Entities;
using Unity.Mathematics;

public struct BeeTargets : IComponentData
{
    public Entity ResourceTarget;
    public float3 HomePosition;
}