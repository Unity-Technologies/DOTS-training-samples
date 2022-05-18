using Unity.Collections;
using Unity.Entities;

struct TileGrid : IComponentData
{
    public int Size;
    public Entity entity;
}
