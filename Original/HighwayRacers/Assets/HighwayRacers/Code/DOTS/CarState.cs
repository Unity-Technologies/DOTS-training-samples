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
