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

    private static DirectionEnum Reverse(DirectionEnum dir)
    {
        return dir switch
        {
            DirectionEnum.North => DirectionEnum.South,
            DirectionEnum.East => DirectionEnum.West,
            DirectionEnum.South => DirectionEnum.North,
            DirectionEnum.West => DirectionEnum.East,
            _ => DirectionEnum.None
        };
    }

    private static int2 DirectionToVector(DirectionEnum dir)
    {
        // Use bitfield to determine direction we are going
        int2 dirVec = new int2(new bool2((dir & DirectionEnum.East) != 0, (dir & DirectionEnum.South) != 0));
        dirVec -= new int2(new bool2((dir & DirectionEnum.West) != 0, (dir & DirectionEnum.North) != 0));
        return dirVec;
    }
    
    private static DirectionEnum SelectNewDirection(DirectionEnum dir)
    {
        var raw = (int)dir << 1;
        return raw > (int)DirectionEnum.West ? DirectionEnum.North : (DirectionEnum)raw;
    }

    private static DirectionEnum GetTileWalls(MapData mapData, DynamicBuffer<TileData> mapTiles, int2 coord)
    {
        if (coord.x < 0 || coord.y < 0 || coord.x >= mapData.Size.x || coord.y >= mapData.Size.y)
        {
            return DirectionEnum.North | DirectionEnum.East | DirectionEnum.South | DirectionEnum.West;
        }
            
        int index = coord.y * mapData.Size.y + coord.x;
        return mapTiles[index].Walls.Value;
    }

    private static int2 Neighbor(int2 coord, DirectionEnum dir)
    {
        return coord + DirectionToVector(dir);
    }
    
    private static bool HasWallOrNeighborWall(MapData mapData, DynamicBuffer<TileData> mapTiles, int2 current, DirectionEnum dir) 
    {
        var walls = GetTileWalls(mapData, mapTiles, current);
        
        if (WallInDirection(walls, dir)) 
        {
            //Debug.Log("cell " + this + " has wall to the " + dir);
            return true;
        }
		
        var neighbor = Neighbor(current, dir);
        var neighborWalls = GetTileWalls(mapData, mapTiles, neighbor);
        var oppositeDirection = Reverse(dir);
        
        if (WallInDirection(neighborWalls, oppositeDirection))
        {
            //Debug.Log("cell " + neighbor + " has wall to the " + oppositeDirection + " (opposite)");
            return true;
        }

        return false;
    }
    
    private static DirectionEnum NextPath(MapData mapData, DynamicBuffer<TileData> mapTiles, int2 current, DirectionEnum dir)
    {
        if (HasWallOrNeighborWall(mapData, mapTiles, current, dir)) {
            dir = SelectNewDirection(dir);
            if (HasWallOrNeighborWall(mapData, mapTiles, current, dir)) {
                dir = Reverse(dir);
                if (HasWallOrNeighborWall(mapData, mapTiles, current, dir)) {
                    dir = Reverse(dir);
                }
            }
        }

        return dir;
    }

    private static void Move(MapData mapData, DynamicBuffer<TileData> mapTiles, ref Tile tile, ref Direction dir, ref TileLerp lerp, float speed, float delta)
    {
        dir.Value = NextPath(mapData, mapTiles, tile.Coords, dir.Value);
        
        lerp.Value += speed * delta;

        if (lerp.Value > 1.0f)
        {
            lerp.Value -= 1.0f;
            
            tile.Coords += DirectionToVector(dir.Value);
            
            dir.Value = NextPath(mapData, mapTiles, tile.Coords, dir.Value);
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