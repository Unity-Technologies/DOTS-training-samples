using System;

using UnityEngine;

using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(10)]
public struct RingElement : IBufferElementData
{
    public float2 offsets;
    public float halfThickness;

    public Opening opening0;
    public Opening opening1;
    public int numberOfOpenings;
}

public struct Opening
{
    public float2 angles;
}
