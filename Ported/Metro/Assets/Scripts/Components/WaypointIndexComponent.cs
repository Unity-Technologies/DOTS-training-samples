using System;
using Unity.Entities;

[Serializable]
public struct WaypointIndexComponent : IComponentData
{
    public int Value;
}