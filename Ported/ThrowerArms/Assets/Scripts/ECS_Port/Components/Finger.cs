using Unity.Entities;
using Unity.Mathematics;

public struct Finger : IComponentData
{
    public float GrabExtent;
    public float3 Target;
}