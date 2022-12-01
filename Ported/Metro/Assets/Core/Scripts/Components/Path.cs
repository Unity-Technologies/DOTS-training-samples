using Unity.Collections;
using Unity.Entities;

struct Path : IComponentData
{
    public Entity Default;
    public Entity EntryLeft;
    public Entity ExitLeft;
    public Entity EntryRight;
    public Entity ExitRight;
}
