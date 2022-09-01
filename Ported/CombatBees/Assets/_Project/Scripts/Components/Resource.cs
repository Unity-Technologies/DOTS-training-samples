using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Resource : IComponentData
{
    public float3 Velocity;
    public int CellIndex;
    public int StackIndex;
}

[Serializable]
public struct ResourceCarrier : IComponentData
{
    public Entity Carrier;
    public byte IsInitialized;
}

[Serializable]
public struct ResourceSettled : IComponentData
{
}

[Serializable]
public struct ResourceInitializeDrop : IComponentData
{
}


[Serializable]
public struct ResourceSnapToCell : IComponentData
{
    public float SnapStartTime;
    public float3 StartPos;
    public float3 TargetPos;
}
