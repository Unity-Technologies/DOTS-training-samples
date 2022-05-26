
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

readonly partial struct PathfindingAspect : IAspect<PathfindingAspect>
{
    public readonly Entity Self;

    public readonly RefRW<PathfindingIntent> PathfindingIntent;
    public readonly RefRO<Translation> Translation;
    public readonly DynamicBuffer<Waypoint> Waypoints;

    public void ClearWaypoints()
    {
        Waypoints.Clear();
    }

    public void AddWaypoint(Waypoint newWaypoint)
    {
        Waypoints.Add(newWaypoint);
    }
}