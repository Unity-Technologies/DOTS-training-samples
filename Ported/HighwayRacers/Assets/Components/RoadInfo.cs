using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct RoadInfo : IComponentData
{
    public float LaneWidth;
    public int MaxLanes;

    //Move this probably
    public float CarSpawningDistancePercent;
    
    public float MidRadius;
    public float StraightPieceLength;

    public int SegmentCount;
}

[InternalBufferCapacity(16)]
public struct LaneInfo : IBufferElementData
{
    // Actual value each buffer element will store.
    public float Pivot;
    public float Radius;
    public float CurvedPieceLength;
    public float TotalLength;
}