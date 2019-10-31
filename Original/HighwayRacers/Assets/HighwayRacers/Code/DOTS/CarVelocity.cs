using System;
using Unity.Entities;

[Serializable]
public struct CarCrossVelocity : IComponentData
{
    public float CrossLaneVel;      // Speed to move between lanes
}

[Serializable]
public struct CarVelocityStaticProperties : IComponentData
{
    public float DefaultSpeed;
    public float MaxSpeed;
    public float Acceleration;
    public float BrakeDecel;
    public float LaneCrossingSpeed;
}
