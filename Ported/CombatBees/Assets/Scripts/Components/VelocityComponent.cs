using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct VelocityComponent : IComponentData
{
    public float3 Value;
}
