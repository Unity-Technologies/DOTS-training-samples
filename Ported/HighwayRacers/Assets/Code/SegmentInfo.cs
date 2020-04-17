using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct SegmentInfo : IComponentData
{
    public float Length;
    public float2 StartXZ;
    public float2 EndXZ;
    public int Order;
    public float2 PercentageRange;
    public float StartRotation;
}

public struct SegmentInfoElement : IBufferElementData
{
    // These implicit conversions are optional, but can help reduce typing.
    public static implicit operator SegmentInfo(SegmentInfoElement e) { return e.Value; }
    public static implicit operator SegmentInfoElement(SegmentInfo e) { return new SegmentInfoElement { Value = e }; }
    
    public SegmentInfo Value;
}
