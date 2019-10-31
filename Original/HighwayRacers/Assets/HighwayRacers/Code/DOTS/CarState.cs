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
public struct CarLogicState : IComponentData
{
    public VehicleState State;
    public float TargetSpeed;
    public float TargetLane;
}

[Serializable]
public struct CarOvertakeStaticProperties : IComponentData
{
    public float OvertakeEagerness;
    public float OvertakeMaxTime;
}
