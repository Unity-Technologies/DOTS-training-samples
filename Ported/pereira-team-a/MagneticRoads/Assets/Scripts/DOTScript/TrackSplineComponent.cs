using System;
using Unity.Entities;
using Unity.Mathematics;

// Serializable attribute is for editor support.
// ReSharper disable once InconsistentNaming
[Serializable]
public struct TrackSplineComponent : IComponentData
{
    public float3 startPoint;
    public float3 endPoint;
    public float3 anchor1;
    public float3 anchor2;
    public float3 startNormal;
    public float3 endNormal;
    public float3 startTangent;
    public float3 endTangent;
}
