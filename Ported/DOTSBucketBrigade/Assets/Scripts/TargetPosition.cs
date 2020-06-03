using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct TargetPosition : IComponentData
{
    public float3 Target;
}

