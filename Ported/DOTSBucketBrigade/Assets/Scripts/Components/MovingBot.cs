using System;
using Unity.Entities;
using Unity.Mathematics;

public struct MovingBot : IComponentData
{
    public float3 StartPosition;
    public float3 TargetPosition;
    public double StartTime;
    public bool HasTagComponentToAddOnArrival;
    public ComponentType TagComponentToAddOnArrival;
}
