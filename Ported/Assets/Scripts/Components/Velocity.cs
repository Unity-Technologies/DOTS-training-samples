using Unity.Entities;
using Unity.Mathematics;

public struct Velocity : IComponentData
{
    public float Speed;
    public float2 Direction;
}