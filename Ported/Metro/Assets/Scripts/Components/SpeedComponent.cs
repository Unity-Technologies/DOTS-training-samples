using System;
using Unity.Entities;

[Serializable]
public struct SpeedComponent : IComponentData
{
    public float Value;
}
