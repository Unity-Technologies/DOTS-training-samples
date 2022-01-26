using Unity.Entities;
using Unity.Mathematics;

public struct TileData : IBufferElementData
{
    public Direction Walls;
}

public struct MapData: IComponentData
{
}
