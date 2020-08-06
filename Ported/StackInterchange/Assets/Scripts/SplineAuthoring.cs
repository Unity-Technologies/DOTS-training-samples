using Unity.Entities;
using Unity.Mathematics;

public struct Spline : IComponentData
{
    public BlobAssetReference<SplineHandle> Value;
}

public struct SplineCategory : IComponentData
{
    public int category;

    public float4 GetColor()
    {
        var color = new float4(1.0f, 0.0f, 1.0f, 1.0f);
        switch (category)
        {
            case 0: color = new float4(1.0f, 0.0f, 0.0f, 1.0f); break; //North, red
            case 1: color = new float4(1.0f, 1.0f, 0.0f, 1.0f); break; //South, yellow
            case 2: color = new float4(0.0f, 1.0f, 0.0f, 1.0f); break; //East, green
            case 3: color = new float4(0.0f, 0.0f, 1.0f, 1.0f); break; //West, blue
        }
        return color;
    }
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

