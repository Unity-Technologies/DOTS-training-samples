using Unity.Entities;
using Unity.Mathematics;

public struct TileData : IBufferElementData
{
    public Direction Walls;
}

public struct MapData: IComponentData
{
    public static DirectionEnum GetTileWalls(in Config config, in DynamicBuffer<TileData> mapTiles, int2 coord)
    {
        if (coord.x < 0 || coord.y < 0 || coord.x >= config.MapWidth || coord.y >= config.MapHeight)
        {
            return DirectionEnum.North | DirectionEnum.East | DirectionEnum.South | DirectionEnum.West;
        }
    
        int index = coord.y * config.MapWidth + coord.x;
        return mapTiles[index].Walls.Value;
    }
    
    public static bool HasHole(in Config config, in DynamicBuffer<TileData> mapTiles, int2 current)
    {
        var hole = GetTileWalls(config, mapTiles, current);
    
        return hole.Impassable();
    }
}

