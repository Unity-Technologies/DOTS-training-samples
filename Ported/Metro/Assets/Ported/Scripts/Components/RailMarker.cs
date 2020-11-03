using System;
using Unity.Entities;
using Unity.Mathematics;

public struct RailMarker : IBufferElementData
{
    public int Index;
    public int MarkerType;
    public float3 Position;
    public float3 HandleIn;
    public float3 HandleOut;
    public float Distance;
}