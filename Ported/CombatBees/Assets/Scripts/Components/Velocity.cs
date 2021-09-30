using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Velocity : IComponentData
{
    public float3 Value;
}