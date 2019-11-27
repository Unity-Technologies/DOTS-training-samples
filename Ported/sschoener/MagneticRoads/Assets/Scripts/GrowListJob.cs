using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
struct GrowListJob : IJob
{
    [ReadOnly]
    public NativeList<TrackSplineCtorData> TrackSplinePrototypes;

    public NativeList<TrackSpline> TrackSplines;
    
    public void Execute() =>
        TrackSplines.ResizeUninitialized(TrackSplinePrototypes.Length);
}
