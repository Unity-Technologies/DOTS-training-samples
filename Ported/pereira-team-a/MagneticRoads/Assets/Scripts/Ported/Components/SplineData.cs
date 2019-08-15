using System;
using Unity.Entities;
using Unity.Mathematics;

public struct SplineData : IComponentData
{
//    public float3 TargetPosition;
//    public float3 StartPosition;
    public Spline Spline;
            
    // Is this a temporary spline, used for intersections
    public bool IsIntersection;
    // If this a temporary spline, what is the next spline it will connect to
    public int TargetSplineId;
}