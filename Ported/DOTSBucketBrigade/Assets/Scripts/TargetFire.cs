using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct TargetFire : IComponentData
{
    public int2 GridIndex;
    public float3 FirePosition;
}