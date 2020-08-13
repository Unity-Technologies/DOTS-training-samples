using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct Bot : IComponentData
{
}
public struct TargetTranslation : IComponentData
{
    public float3 Value;
}