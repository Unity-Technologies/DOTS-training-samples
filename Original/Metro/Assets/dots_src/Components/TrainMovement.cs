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

public struct TrainMovement : IComponentData
{
    public TrainMovemementStates state;
    public float speed;
    public float position;
    public double timeWhenStoppedAtPlatform; // TODO: just for stopping train, will be removed later
}
