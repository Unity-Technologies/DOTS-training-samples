using Unity.Entities;
using Unity.Mathematics;

public struct Drone : IComponentData
{
    public float3 smoothPosition;
    public float hoverHeight;
    public float2 storePosition;
    public float searchTimer;
    public float moveSmooth;
    public float3 destination;
}