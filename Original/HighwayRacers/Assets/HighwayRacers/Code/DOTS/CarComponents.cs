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
public struct CarBasicState : IComponentData
{
    public float Lane; // [0..4) //?
    public float Position; // Position on the road.
    public float Speed;
}

[Serializable]
public struct CarLogicState : IComponentData
{
    public VehicleState State;
    public float TargetLane;
    public int OvertakingCarIndex;
    public float OvertakeRemainingTime;
}

[Serializable]
public struct CarReadOnlyProperties : IComponentData
{
    public float DefaultSpeed;
    public float MaxSpeed;
    public float MergeDistance;
    public float MergeSpace;
    public float OvertakeEagerness;
}
