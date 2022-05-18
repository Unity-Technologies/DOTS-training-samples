using Unity.Entities;
using Unity.Mathematics;

struct Tile : IComponentData
{
    public int2 Position;
    public float4 Color;
    public float Height;
    public float Heat;
    public float Water;
}
