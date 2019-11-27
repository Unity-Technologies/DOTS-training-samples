using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public struct UninitializedComponentTag : IComponentData{}

public struct CarSpawnComponent : IComponentData
{
    public int Count;
}

public struct CarSetupTagComponent : IComponentData
{
}

public struct CarSpeedComponent : IComponentData
{
    public float NormalizedSpeed;
    public float SplineTimer;
}

public struct SplineDataComponent : IComponentData
{
    public CubicBezier Bezier;
    public TrackGeometry Geometry;
    public float Length;
    public byte TwistMode;
}

public enum VehicleState : byte
{
    OnRoad = 0,
    OnIntersection = 1,
    EnteringIntersection = 2,
}

public struct SplinePosition
{
    public ushort Spline;
    public sbyte State;

    public sbyte Side
    {
        get => (sbyte)((State & 0x1) > 0 ? 1 : -1);
        set
        {
            int v = value > 0 ? 1 : 0;
            State = (sbyte)((State & ~0x1) | v);
        }
    }
    public sbyte Direction
    {
        get => (sbyte)((State & 0x2) > 0 ? 1 : -1);
        set
        {
            int v = value > 0 ? 2 : 0;
            State = (sbyte)((State & ~0x2) | v);
        }
    }

    public bool Dirty
    {
        get => (State & 0x10) == 0;
        set
        {
            int v = value ? 0 : 0x10;
            State = (sbyte)((State & ~0x10) | v);
        }
    }

    public static SplinePosition Invalid => new SplinePosition { Spline = UInt16.MaxValue };
}

public struct VehicleStateComponent : IComponentData
{
    public VehicleState Value;

    public static implicit operator VehicleState(VehicleStateComponent vsc) => vsc.Value;
}

public struct OnSplineComponent : IComponentData
{
    public SplinePosition Value;
}

public struct LocalIntersectionComponent : IComponentData
{
    public CubicBezier Bezier;
    public TrackGeometry Geometry;
    public float Length;
    public SplinePosition FromSpline;
    public sbyte Side;
}

public struct CoordinateSystemComponent : IComponentData
{
    public float3 Position;
    public float3 Up;
    public float3 Forward;
}

struct RoadSetupComponent : IComponentData
{
    public BlobAssetReference<TrackSplinesBlob> Splines;
    public BlobAssetReference<IntersectionsBlob> Intersections;
    public float IntersectionSize;
    public float TrackRadius;
    public float TrackThickness;
    public int SplineResolution;
    public int TrisPerMesh;
}

struct IntersectionComponent : IComponentData
{
    public ushort Index;
}

[Serializable]
[MaterialProperty("_Color", MaterialPropertyFormat.Float4)]
public struct CarColor : IComponentData
{
    public float4 Value;
}