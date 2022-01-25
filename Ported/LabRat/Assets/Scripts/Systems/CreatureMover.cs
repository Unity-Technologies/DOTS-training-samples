using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(MapSpawningSystem))]
public partial class CreatureMover : SystemBase
{
    private static bool WallInDirection(DirectionEnum walls, DirectionEnum dir)
    {
        return (walls & dir) != 0;
    }
    
    private static DirectionEnum NextDirection(DirectionEnum dir)
    {
        var raw = (int)dir << 1;
        return raw > (int)DirectionEnum.West ? DirectionEnum.North : (DirectionEnum)raw;
    }

    private static DirectionEnum GetTileWalls(MapData mapData, DynamicBuffer<TileData> mapTiles, int2 coord)
    {
        //TODO: Add range check if in map

        int index = coord.y * mapData.Size.y + coord.x;
        return mapTiles[index].Walls.Value;
    }

    private static DirectionEnum NextPath(DirectionEnum walls, DirectionEnum dir)
    {
        int tries = 0;
        while (WallInDirection(walls, dir) && tries < 4)
        {
            dir = NextDirection(dir);
            tries += 1;
        }
        return tries == 4 ? DirectionEnum.None : dir;
    }

    private static void Move(MapData mapData, DynamicBuffer<TileData> mapTiles, ref Tile tile, ref Direction dir, ref TileLerp lerp, float speed, float delta)
    {
        // Get walls at current coords
        DirectionEnum walls = GetTileWalls(mapData, mapTiles, tile.Coords);

        dir.Value = NextPath(walls, dir.Value);
            
        lerp.Value += speed * delta;

        // Use bitfield to determine direction
        int2 direction = new int2(new bool2((dir.Value & DirectionEnum.East) != 0, (dir.Value & DirectionEnum.South) != 0));
        direction -= new int2(new bool2((dir.Value & DirectionEnum.West) != 0, (dir.Value & DirectionEnum.North) != 0));

        if (lerp.Value > 1.0f)
        {
            lerp.Value -= 1.0f;
            
            tile.Coords += direction;

            if (tile.Coords.x < 0 || tile.Coords.x >= 20 || tile.Coords.y < 0 || tile.Coords.y >= 20)
            {
                tile.Coords -= direction;
                dir.Value = NextDirection(dir.Value);
            }
        }
    }

    protected override void OnUpdate()
    {
        Config conf = GetSingleton<Config>();
        MapData mapData = GetSingleton<MapData>();
        DynamicBuffer<TileData> mapTiles = GetBuffer<TileData>(GetSingletonEntity<MapData>());
        var delta = Time.DeltaTime;

        Entities
            .WithAll<Mouse>()
            .WithReadOnly(mapTiles)
            .ForEach((ref Tile tile, ref Direction dir, ref TileLerp lerp) =>
            {
                Move(mapData, mapTiles, ref tile, ref dir, ref lerp, conf.MouseMovementSpeed, delta);
            }).ScheduleParallel();

        Entities
            .WithAll<Cat>()
            .WithReadOnly(mapTiles)
            .ForEach((ref Tile tile, ref Direction dir, ref TileLerp lerp) =>
            {
                Move(mapData, mapTiles, ref tile, ref dir, ref lerp, conf.CatMovementSpeed, delta);
            }).ScheduleParallel();

    }
}