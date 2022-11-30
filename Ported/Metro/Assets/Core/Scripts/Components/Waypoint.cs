using Unity.Collections;
using Unity.Entities;

struct Waypoint : IComponentData
{
    public int PathID;
    public int WaypointID;
    public Entity WaypointEntity;
    public Entity NextWaypointEntity;
}
