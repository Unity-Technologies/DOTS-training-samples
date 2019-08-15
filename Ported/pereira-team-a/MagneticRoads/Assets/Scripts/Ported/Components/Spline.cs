using Unity.Entities;
using Unity.Mathematics;

public struct Spline : IBufferElementData
{
    public int EndIntersectionId; //id to find an element in the intersection data
    public float3 Anchor1;
    public float3 Anchor2;
    public float3 StartNormal;
    public float3 EndNormal;
    public float3 StartTargent;
    public float3 EndTangent;
}