using System;
using Unity.Entities;
using Unity.Mathematics;

public struct SplineData : IComponentData
{
    public float3 TargetPosition;
    public float3 StartPosition;
    public Spline spline;
}