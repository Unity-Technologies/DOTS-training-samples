using Unity.Entities;
using Unity.Mathematics;

public unsafe struct IntersectionBufferElementData : IBufferElementData
{
    public float3 Position; // Position of intersection point in 3d-space
    public float3 Normal; // Position of intersection point in 3d-space
    //public fixed int Neighbors[3];  // ID of Spline in Dynamic Buffer

    public int SplineIdCount;  // Number of neighbors
    public int SplineId0;  // ID of Spline in Dynamic Buffer
    public int SplineId1;  // ID of Spline in Dynamic Buffer
    public int SplineId2;  // ID of Spline in Dynamic Buffer

    public int LastIntersection; // Used to assign intersections
}