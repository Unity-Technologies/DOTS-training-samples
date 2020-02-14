using Unity.Entities;
using Unity.Mathematics;

public struct MovementDerivatives : IComponentData
{
    public float3 Speed;
    public float Acceleration;
}
