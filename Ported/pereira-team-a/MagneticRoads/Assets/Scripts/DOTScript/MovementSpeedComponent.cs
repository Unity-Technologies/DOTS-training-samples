using System;
using Unity.Entities;
using Unity.Mathematics;

// Serializable attribute is for editor support.
// ReSharper disable once InconsistentNaming
[Serializable]
public struct MovementSpeedComponent : IComponentData
{
    public float speed;
    public float3 lastPosition;
}
