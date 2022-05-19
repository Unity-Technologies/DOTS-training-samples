using Unity.Entities;
using Unity.Mathematics;

public struct BezierPoint
{
    public int index;
    public float3 location, handle_in, handle_out;
    public float distanceAlongPath;
}

public struct BezierData
{
    public BlobArray<BezierPoint> Points;
    public float distance;
}

public struct BezierPath : IComponentData
{
    public BlobAssetReference<BezierData> Data;
}