using Unity.Entities;
using Unity.Mathematics;

public struct MovementParabola : IComponentData
{
    public float3 Origin;
    public float3 Target;
    public float3 Parabola;
}
