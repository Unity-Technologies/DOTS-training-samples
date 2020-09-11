using Unity.Entities;
using Unity.Mathematics;

public struct AIMousePositionTarget : IComponentData
{
    public float2 Value;
    public int2 Tile;
    public Direction.Attributes Direction;
}