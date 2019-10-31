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
    public float TargetLane;
    public int OvertakingCarIndex;
    public float OvertakeRemainingTime;
}
