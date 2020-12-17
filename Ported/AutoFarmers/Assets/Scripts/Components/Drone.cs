using Unity.Entities;
using Unity.Mathematics;

public struct Drone : IComponentData
{
    public float3 smoothPosition;
    public float moveSmooth;
}