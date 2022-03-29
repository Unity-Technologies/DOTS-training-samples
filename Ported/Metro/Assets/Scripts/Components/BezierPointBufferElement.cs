using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(16)]
// Represents a Bezier Point
public struct BezierPointBufferElement : IBufferElementData
{
    public float3 Location;
    public float3 HandleIn;
    public float3 HandleOut;

    public float DistanceAlongPath;
}
