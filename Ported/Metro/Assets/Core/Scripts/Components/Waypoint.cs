using Unity.Entities;

struct Waypoint : IComponentData
{
    public int PathID;
    public int WaypointID;
}
