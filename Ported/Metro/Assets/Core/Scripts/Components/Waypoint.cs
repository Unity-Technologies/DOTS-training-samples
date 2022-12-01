using Unity.Collections;
using Unity.Entities;

struct Waypoint : IComponentData
{
    public Entity PathEntity;
    public Entity NextWaypointEntity;
    public Entity Connection;
}
