using Unity.Entities;
using Unity.Mathematics;

public struct Spline : IComponentData
{
    public BlobAssetReference<SplineHandle> Value;
}

public struct SplineHandle
{
    public BlobArray<int> Segments; //int offset in segment collection
}

public struct SegmentCollection : IComponentData //Singleton
{
    public BlobAssetReference<SegmentHandle> Value;
}

public struct SegmentHandle
{
    public BlobArray<SegmentData> Segments;
}

public struct SegmentData
{
    public float3 Start;
    public float3 End;
}

