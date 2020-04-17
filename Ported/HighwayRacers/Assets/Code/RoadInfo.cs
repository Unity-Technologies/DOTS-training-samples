using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct RoadInfo : IComponentData
{
    public float2 StartXZ;
    public float2 EndXZ;
    public float TotalLength;
    public float LaneWidth;
    public int MaxLanes;

    //Move this probably
    public float CarSpawningDistancePercent;
    public float MidRadius;

    public int SegmentCount;
}

public struct LaneInfo
{
    public float Pivot;
    public float Radius;
}

// This describes the number of buffer elements that should be reserved
// in chunk data for each instance of a buffer. In this case, 8 integers
// will be reserved (32 bytes) along with the size of the buffer header
// (currently 16 bytes on 64-bit targets)
[InternalBufferCapacity(8)]
public struct LaneInfoElement : IBufferElementData
{
    // These implicit conversions are optional, but can help reduce typing.
    public static implicit operator LaneInfo(LaneInfoElement e) { return e.Value; }
    public static implicit operator LaneInfoElement(LaneInfo e) { return new LaneInfoElement { Value = e }; }

    // Actual value each buffer element will store.
    public LaneInfo Value;
}