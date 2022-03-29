using System;
using Unity.Entities;

[Serializable]
public struct TrackPositionComponent : IComponentData
{
    public float Value;
}