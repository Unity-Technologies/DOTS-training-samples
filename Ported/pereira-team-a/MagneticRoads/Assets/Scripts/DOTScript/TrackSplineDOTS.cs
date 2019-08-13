using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;

public struct TrackSplineDOTS
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

    public NativeArray<TrackSplineDOTS> neighbors;
}
