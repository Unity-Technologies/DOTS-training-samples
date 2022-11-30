using Unity.Collections;
using Unity.Entities;

struct Path : IComponentData
{
    public Entity Default;
    public Entity Entry;
    public Entity Exit;
}
