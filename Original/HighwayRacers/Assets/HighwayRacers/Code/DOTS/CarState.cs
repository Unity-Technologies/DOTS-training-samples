using System;
using Unity.Entities;

public enum VehicleState
{
    NORMAL,
    MERGE_RIGHT,
    MERGE_LEFT,
    OVERTAKING,
}

[Serializable]
public struct CarState : IComponentData
{
    public VehicleState state;
}

[Serializable]
public struct CarOvertakeStaticProperties : IComponentData
{
    public float OvertakeEagerness;
    public float OvertakeMaxTime;
}

[Serializable]
public struct CarMergeStaticProperties : IComponentData
{
    public float MergeDistance;
    public float MergeSpace;
}