using System;
using Unity.Entities;

[Serializable]
public struct SpeedComponent : IComponentData
{
    public float Acceleration;
    public float CurrentSpeed;
    public float MaxSpeed;
}
