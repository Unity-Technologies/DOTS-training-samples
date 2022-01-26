using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateAfter(typeof(MapSpawningSystem))]
public partial class CreatureMovementSystem : SystemBase
{
    private static bool WallInDirection(DirectionEnum walls, DirectionEnum dir)
    {
        return (walls & dir) != 0;
    }
    
    private static DirectionEnum SelectNewDirection(DirectionEnum dir)
    {
        var raw = (int)dir << 1;
        return raw > (int)DirectionEnum.West ? DirectionEnum.North : (DirectionEnum)raw;
    }

    private static int2 Neighbor(int2 coord, DirectionEnum dir)
    {
        return coord + dir.ToVector2();
    }
    
    private static bool HasWallOrNeighborWall(Config config, DynamicBuffer<TileData> mapTiles, int2 current, DirectionEnum dir) 
    {
        var walls = MapData.GetTileWalls(config, mapTiles, current);

        if (WallInDirection(walls, dir)) 
        {
            //Debug.Log("cell " + this + " has wall to the " + dir);
            return true;
        }
		
        var neighbor = Neighbor(current, dir);
        var neighborWalls = MapData.GetTileWalls(config, mapTiles, neighbor);
        var oppositeDirection = dir.Reverse();
        
        if (WallInDirection(neighborWalls, oppositeDirection))
        {
            //Debug.Log("cell " + neighbor + " has wall to the " + oppositeDirection + " (opposite)");
            return true;
        }

        return false;
    }
    
    private static DirectionEnum NextPath(Config conf, DynamicBuffer<TileData> mapTiles, int2 current, DirectionEnum dir)
    {
        if (MapData.HasHole(conf, mapTiles, current))
        {
            return DirectionEnum.Hole;
        }
        
        if (HasWallOrNeighborWall(conf, mapTiles, current, dir)) {
            dir = SelectNewDirection(dir);
            if (HasWallOrNeighborWall(conf, mapTiles, current, dir)) {
                dir = dir.Reverse();
                if (HasWallOrNeighborWall(conf, mapTiles, current, dir)) {
                    dir = dir.Reverse();
                }
            }
        }

        return dir;
    }

    private static void Move(Config conf, DynamicBuffer<TileData> mapTiles, ref Tile tile, ref Direction dir, ref TileLerp lerp, float speed, float delta)
    {
        dir.Value = NextPath(conf, mapTiles, tile.Coords, dir.Value);

        if (dir.Value == DirectionEnum.Hole)
        {
            lerp.Value += conf.CreatureFallSpeed * delta;
            return;
        }

        lerp.Value += speed * delta;

        if (lerp.Value > 1.0f)
        {
            lerp.Value -= 1.0f;
            
            tile.Coords += dir.Value.ToVector2();
            
            dir.Value = NextPath(conf, mapTiles, tile.Coords, dir.Value);
        }
    }

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameRunning>();
    }

    protected override void OnUpdate()
    {
        Config conf = GetSingleton<Config>();
        DynamicBuffer<TileData> mapTiles = GetBuffer<TileData>(GetSingletonEntity<MapData>());
        var delta = Time.DeltaTime;

        Entities
            .WithAll<Mouse>()
            .WithReadOnly(mapTiles)
            .ForEach((ref Tile tile, ref Direction dir, ref TileLerp lerp) =>
            {
                Move(conf, mapTiles, ref tile, ref dir, ref lerp, conf.MouseMovementSpeed, delta);
            }).ScheduleParallel();

        Entities
            .WithAll<Cat>()
            .WithReadOnly(mapTiles)
            .ForEach((ref Tile tile, ref Direction dir, ref TileLerp lerp) =>
            {
                Move(conf, mapTiles, ref tile, ref dir, ref lerp, conf.CatMovementSpeed, delta);
            }).ScheduleParallel();

    }
}