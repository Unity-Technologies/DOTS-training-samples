using Unity.Entities;
using UnityEngine;

public enum TrainMovementStates
{
    Starting,
    Running,
    Stopping,
    Waiting,
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
    /// distance to station on train stop in unit points
    /// </summary>
    public float distanceToStop;
}
