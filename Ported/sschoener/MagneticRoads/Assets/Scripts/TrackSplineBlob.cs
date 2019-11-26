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

    [DeallocateOnJobCompletion]
    public NativeArray<CubicBezier> Bezier;
    [DeallocateOnJobCompletion]
    public NativeArray<TrackGeometry> Geometry;
    [DeallocateOnJobCompletion]
    public NativeArray<ushort> EndIntersection;
    [DeallocateOnJobCompletion]
    public NativeArray<ushort> StartIntersection;
    [DeallocateOnJobCompletion]
    public NativeArray<byte> TwistMode;
    [DeallocateOnJobCompletion]
    public NativeArray<float> CarQueueSize;
    [DeallocateOnJobCompletion]
    public NativeArray<float> MeasuredLength;
    [DeallocateOnJobCompletion]
    public NativeArray<int> MaxCarCount;

    public void Execute(int index)
    {
        BlobArray[index].Bezier = Bezier[index];
        BlobArray[index].Geometry = Geometry[index];
        BlobArray[index].EndIntersection = EndIntersection[index];
        BlobArray[index].StartIntersection = StartIntersection[index];
        BlobArray[index].TwistMode = TwistMode[index];
        BlobArray[index].CarQueueSize = CarQueueSize[index];
        BlobArray[index].MeasuredLength = MeasuredLength[index];
        BlobArray[index].MaxCarCount = MaxCarCount[index];
    }
}