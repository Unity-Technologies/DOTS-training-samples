using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct TargetPoint : IComponentData
{
    public float3 CurrentTarget;
}
