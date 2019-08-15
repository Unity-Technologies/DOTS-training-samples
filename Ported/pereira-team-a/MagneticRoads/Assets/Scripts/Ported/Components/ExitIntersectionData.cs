using System;
using Unity.Entities;
using Unity.Mathematics;

public struct ExitIntersectionData : IComponentData
{
    // If this a temporary spline, what is the next spline it will connect to
    public int TargetSplineId;
}