using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;

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

unsafe struct BuildTrackSplineBlobJob : IJobParallelFor
{
    [NativeDisableUnsafePtrRestriction]
    public TrackSpline* BlobArray;
    
    public NativeArray<TrackSpline> TrackSplines;
    [DeallocateOnJobCompletion]
    public NativeArray<byte> TwistMode;

    public void Execute(int index)
    {
        BlobArray[index] = TrackSplines[index];
        BlobArray[index].TwistMode = TwistMode[index];
    }
}