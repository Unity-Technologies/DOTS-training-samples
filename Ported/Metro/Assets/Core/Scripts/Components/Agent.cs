using Unity.Collections;
using Unity.Entities;

struct Agent : IComponentData
{
    public Entity CurrentWaypoint;
}
