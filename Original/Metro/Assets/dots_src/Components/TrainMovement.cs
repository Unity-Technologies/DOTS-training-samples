using Unity.Entities;
using UnityEngine;

public enum TrainMovemementStates
{
    Starting = 1,
    Running = 2,
    Stopping = 3,
    Stopped = 4,
    Waiting = 5,
}

[GenerateAuthoringComponent]
public struct TrainMovement : IComponentData
{
    public TrainMovemementStates state;
    
    /// <summary>
    /// Speed in m/s
    /// </summary>
    public float speed;
    
    /// <summary>
    /// distance to station on train stop in unit points
    /// </summary>
    public float distanceToStop;

    /// <summary>
    /// How much time in seconds before setting state to start
    /// </summary>
    public float restingTimeLeft; // TODO: just for stopping train, will be removed later
}
