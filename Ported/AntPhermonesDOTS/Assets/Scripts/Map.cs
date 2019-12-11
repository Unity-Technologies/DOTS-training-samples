using Unity.Entities;
using Unity.Mathematics;

struct Map : IComponentData
{
    public int Size;
}

struct Tile : IComponentData
{
    public int2 Coordinates;
}
