using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct MovementDerivatives : IComponentData
{
    public float3 Speed;
    public float Acceleration;
}
