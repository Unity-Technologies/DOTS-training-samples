using System;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct WaypointIndexComponent : IComponentData
{
    public int LastWaypoint;
    public int NextWaypoint;
}