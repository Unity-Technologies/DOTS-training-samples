using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct SteeringAngle : IComponentData
{
    public float3 value;
}