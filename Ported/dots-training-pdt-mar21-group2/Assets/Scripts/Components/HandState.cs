using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Not used anymore (at least for now)
/// </summary>
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
