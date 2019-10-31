using System;
using Unity.Entities;

[Serializable]
public struct CarCrossVelocity : IComponentData
{
    public float CrossLaneVel;      // Speed to move between lanes
}
