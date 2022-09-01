using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Resource : IComponentData
{
    public float3 Velocity;
}

[Serializable]
public struct ResourceCarrier : IComponentData
{
    public Entity Carrier;
}

[Serializable]
public struct ResourceSettled : IComponentData
{
    public int StackIndex;
    public int CellIndex;
}

[Serializable]
public struct ResourceSnapToCell : IComponentData
{
    public float SnapStartTime;
    public float3 StartPos;
    public float3 TargetPos;
}
