using Unity.Entities;
using Unity.Mathematics;

public struct Tile: IComponentData
{
    public int2 Coords;

    public Tile(int2 coordinate)
    {
        Coords = coordinate;
    }
}
