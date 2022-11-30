using Unity.Collections;
using Unity.Entities;

struct Waypoint : IComponentData
{
    public int WaypointID;
    public Entity PathEntity;
    public Entity NextWaypointEntity;
    public Entity Connection;
}
