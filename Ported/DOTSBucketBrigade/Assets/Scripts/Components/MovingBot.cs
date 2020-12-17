using System;
using Unity.Entities;
using Unity.Mathematics;

public struct MovingBot : IComponentData
{
    public float2 StartPosition;
    public float2 TargetPosition;
    public float StartTime;
    public int NewStateOnArrival;
}
