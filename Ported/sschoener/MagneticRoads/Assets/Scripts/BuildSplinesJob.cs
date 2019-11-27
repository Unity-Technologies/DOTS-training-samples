using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
struct BuildSplinesJob : IJobParallelForDefer
{
    public float CarSpacing;
    public int SplineResolution;
    public float IntersectionSize;
    
    [ReadOnly]
    public NativeList<TrackSplineCtorData> TrackSplinePrototypes;

    [NativeDisableParallelForRestriction]
    public NativeList<TrackSpline> OutTrackSplines;
    [ReadOnly]
    public NativeList<Intersection> Intersections;
    
    public void Execute(int index)
    {
        ushort start = TrackSplinePrototypes[index].startIntersection;
        ushort end = TrackSplinePrototypes[index].endIntersection;
        var b = new CubicBezier();
        var startP = b.start = Intersections[start].Position + .5f * IntersectionSize * TrackSplinePrototypes[index].tangent1;
        var endP = b.end = Intersections[end].Position + .5f * IntersectionSize * TrackSplinePrototypes[index].tangent2;

        float dist = math.length(startP - endP);
        b.anchor1 = startP + .5f * dist * TrackSplinePrototypes[index].tangent1;
        b.anchor2 = endP + .5f * dist * TrackSplinePrototypes[index].tangent2;
        var g = new TrackGeometry();
        g.startTangent = math.round(TrackSplinePrototypes[index].tangent1);
        g.endTangent = math.round(TrackSplinePrototypes[index].tangent2);
        g.startNormal = Intersections[start].Normal;
        g.endNormal = Intersections[end].Normal;

        var measuredLength = b.MeasureLength(SplineResolution);
        var maxCarCount = (int)math.ceil(measuredLength / CarSpacing);

        OutTrackSplines[index] = new TrackSpline
        {
            StartIntersection = TrackSplinePrototypes[index].startIntersection,
            EndIntersection = TrackSplinePrototypes[index].endIntersection,
            Bezier = b,
            Geometry = g,
            MeasuredLength = measuredLength,
            MaxCarCount = maxCarCount,
            CarQueueSize = 1f / maxCarCount
        };
    }
}
