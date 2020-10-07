using Unity.Entities;
using Unity.Mathematics;

public struct AICursor : IComponentData
{
    public float2 Destination;
}