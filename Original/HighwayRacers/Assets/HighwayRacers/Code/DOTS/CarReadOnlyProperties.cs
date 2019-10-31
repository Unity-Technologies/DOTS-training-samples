using System;
using Unity.Entities;

[Serializable]
public struct CarReadOnlyProperties : IComponentData
{
    public float DefaultSpeed;
    public float MaxSpeed;
    public float LaneCrossingSpeed;
    public float MergeDistance;
    public float MergeSpace;
}
