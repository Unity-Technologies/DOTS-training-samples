using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct SegmentInfo : IComponentData
{
    public float2 StartXZ;
    public float StartRotation;
    
    public int Order;
    public SegmentShape SegmentShape;
}

public enum SegmentShape
{
    Straight,
    Curved
}

public struct SegmentInfoElement : IBufferElementData
{
    public Entity Entity;   
    public SegmentInfo SegmentInfo;
}

public struct LanePercentageRangeElement : IBufferElementData
{
    // These implicit conversions are optional, but can help reduce typing.
    public static implicit operator float2(LanePercentageRangeElement e) { return e.Value; }
    public static implicit operator LanePercentageRangeElement(float2 e) { return new LanePercentageRangeElement { Value = e }; }
    
    public float2 Value;
}
