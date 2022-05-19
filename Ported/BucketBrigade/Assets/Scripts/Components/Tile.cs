using Unity.Entities;
using Unity.Mathematics;

struct Tile : IComponentData
{
    public int2 Position;
    public float Water;
}
