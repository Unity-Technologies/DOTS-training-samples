using Unity.Entities;

public struct TrackSpline
{
    public CubicBezier Bezier;
    public TrackGeometry Geometry;
    public float MeasuredLength;
    public float CarQueueSize;
    public ushort StartIntersection;
    public ushort EndIntersection;
    public int MaxCarCount;
    public byte TwistMode;
}

public struct TrackSplinesBlob
{
    public BlobArray<TrackSpline> Splines;

    public static BlobAssetReference<TrackSplinesBlob> Instance;
}