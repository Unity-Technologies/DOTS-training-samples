using System.Collections;
using System.Collections.Generic;
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
    /// Position on track, relative to total length of points on track, e.g. position 800 of 1000 track points
    /// </summary>
    public float position;

    public float distanceToStation;
    
    public float restingTimeLeft; // TODO: just for stopping train, will be removed later
}
