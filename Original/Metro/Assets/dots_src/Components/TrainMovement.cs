using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public enum TrainMovementStates
{
    Starting = 1,
    Running = 2,
    Stopping = 3,
    Stopped = 4,
    Waiting = 5,
}

public struct TrainState : IComponentData
{
    public TrainMovementStates State;
    public static implicit operator TrainMovementStates(TrainState _trainState) => _trainState.State;
}

[GenerateAuthoringComponent]
public struct TrainMovement : IComponentData
{
    /// <summary>
    /// Speed in m/s
    /// </summary>
    public float speed;
    
    /// <summary>
    /// Position on track is defined in unit points
    /// </summary>
    public float position;

    /// <summary>
    /// distance to station on train stop in unit points
    /// </summary>
    public float distanceToStation;
}
