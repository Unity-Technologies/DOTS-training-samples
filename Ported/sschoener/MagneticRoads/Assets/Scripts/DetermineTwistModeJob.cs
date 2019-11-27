using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

[BurstCompile]
unsafe struct DetermineTwistModeJob : IJobParallelForDefer
{
    public int Resolution;
    [NativeDisableParallelForRestriction]
    public NativeList<TrackSpline> Splines;

    public void Execute(int index)
    {
        byte twistMode = TrackUtils.SelectTwistMode(Splines[index].Bezier, Splines[index].Geometry, Resolution);
        ((TrackSpline*)Splines.GetUnsafePtr())[index].TwistMode = twistMode;
    }
}
