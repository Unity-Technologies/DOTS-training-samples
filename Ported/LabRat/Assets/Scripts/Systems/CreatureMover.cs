using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class CreatureMover : SystemBase
{
    private static DirectionEnum NextDirection(DirectionEnum dir)
    {
        var raw = (int)dir << 1;
        return raw > (int)DirectionEnum.West ? DirectionEnum.North : (DirectionEnum)raw;
    }

    private static void Move(ref Tile tile, ref Direction dir, ref TileLerp lerp, ref Translation trans, float speed, float delta)
    {
        lerp.Value += speed * delta;

        // Use bitfield to determine direction
        int2 direction = new int2(new bool2((dir.Value & DirectionEnum.East) != 0, (dir.Value & DirectionEnum.South) != 0));
        direction -= new int2(new bool2((dir.Value & DirectionEnum.West) != 0, (dir.Value & DirectionEnum.North) != 0));

        if (lerp.Value > 1.0f)
        {
            lerp.Value -= 1.0f;

            // TODO: Check playing field boundaries (probably already done with wall check if boundary has one)
            // TODO: Add wall check

            tile.Coords += direction;

            if (tile.Coords.x < 0 || tile.Coords.x >= 20 || tile.Coords.y < 0 || tile.Coords.y >= 20)
            {
                tile.Coords -= direction;
                dir.Value = NextDirection(dir.Value);
            }
        }

        // Update translation for rendering
        trans.Value.xz = tile.Coords + lerp.Value * new float2(direction);
    }

    protected override void OnUpdate()
    {
        Config conf = GetSingleton<Config>();
        var delta = Time.DeltaTime;

        Entities
            .WithAll<Mouse>()
            .ForEach((ref Tile tile, ref Direction dir, ref TileLerp lerp, ref Translation trans) =>
            {
                Move(ref tile, ref dir, ref lerp, ref trans, conf.MouseMovementSpeed, delta);
            }).ScheduleParallel();

        Entities
            .WithAll<Cat>()
            .ForEach((ref Tile tile, ref Direction dir, ref TileLerp lerp, ref Translation trans) =>
            {
                Move(ref tile, ref dir, ref lerp, ref trans, conf.CatMovementSpeed, delta);
            }).ScheduleParallel();
    }
}