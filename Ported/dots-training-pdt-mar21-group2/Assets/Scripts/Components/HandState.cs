using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct HandState : IComponentData
{
    public enum State
    {
        Idle,
        Grabbing,
        Throwing
    }

    public State Value;
}
