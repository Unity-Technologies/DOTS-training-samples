using Unity.Entities;
using Unity.Mathematics;

public struct TileComponent : IComponentData
{

    public int2 coord;

    public TileComponent(int2 coordinate)
    {
        coord = coordinate;
    }
}
