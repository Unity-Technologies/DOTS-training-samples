using Unity.Entities;
using UnityEngine;

public enum TrainMovementStates
{
    Starting = 1,
    Running = 2,
    Stopping = 3,
    Stopped = 4,
    WaitingAtPlatform = 5,
    WaitingBehindTrain = 6,
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
    /// distance to station on train stop in unit points, from when we initiate stopping
    /// so we can calculate speed
    /// </summary>
    public float distanceToStop;

    /// <summary>
    /// Keep track of where we want to stop
    /// </summary>
    public float stopPosition;

    /// <summary>
    /// What waiting state will we enter when stopped? either WaitingAtPlatfor or WaitingBehindTrain
    /// </summary>
    public TrainMovementStates WaitingForState;
}
