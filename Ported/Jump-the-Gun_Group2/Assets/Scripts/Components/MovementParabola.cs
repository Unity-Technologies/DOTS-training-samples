using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct MovementParabola : IComponentData
{
    public float3 Origin;
    public float3 Target;
    public float3 Parabola;
}
