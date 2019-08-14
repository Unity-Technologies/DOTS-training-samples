using System;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Entities;

[Serializable]
public struct TrackSplineComponentData : IComponentData
{
    public float3 startPoint;
    public float3 endPoint;
    public float3 anchor1;
    public float3 anchor2;
    public float measuredLength;
    public int3 startNormal;
    public int3 endNormal;
    public int3 startTangent;
    public int3 endTangent;

    //public NativeArray<TrackSplineComponentData> Neighbors;
}
