using Unity.Entities;
using Unity.Mathematics;

public struct Spline : IBufferElementData
{
    // Position
    public float3 StartPosition;
    public float3 EndPosition;
    // Anchors
    public float3 Anchor1;
    public float3 Anchor2;
    // Normals
    public float3 StartNormal;
    public float3 EndNormal;
    // Tangents
    public float3 StartTangent;
    public float3 EndTangent;
    
    // ID of intersection where the spline ends
    public int EndIntersectionId; // id to find an element in the intersection data
}