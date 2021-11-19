using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct TargetOriginalPosition : IComponentData
{
    public float3 Value;
}