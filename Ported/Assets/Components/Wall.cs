using Unity.Entities;
using Unity.Mathematics;

public struct Wall : IComponentData
{
    public float2 Angles;
    public float Radius;
}