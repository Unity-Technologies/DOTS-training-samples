
using Unity.Entities;
using UnityEngine;

public struct PathfindingIntent : IComponentData
{
    public NavigatorType navigatorType;
    public PathfindingDestination destinationType;
    public RectInt RequiredZone;
}
