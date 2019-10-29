using System;
using Unity.Entities;

[Serializable]
public struct CarVelocity : IComponentData
{
    public float InLaneVel;         // Speed within the lane
    public float CrossLaneVel;      // Speed to move between lanes
}

