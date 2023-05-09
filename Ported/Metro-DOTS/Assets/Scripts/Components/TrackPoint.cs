using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[InternalBufferCapacity(200)]
public struct TrackPoint : IBufferElementData
{
    public bool IsStation;
    public bool IsEnd;
    public float3 Position;
}
