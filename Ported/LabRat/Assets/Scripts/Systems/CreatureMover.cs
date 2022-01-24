using System;
using Unity.Entities;
using Unity.Mathematics;

public partial class CreatureMover : SystemBase
{
    protected override void OnUpdate()
    {
        Config conf = GetSingleton<Config>();
        var delta = Time.DeltaTime;

        Entities
            .WithAll<Mouse>()
            .ForEach((ref Tile tile, ref Direction dir, ref TileLerp lerp) =>
            {
                lerp.Value += conf.MouseMovementSpeed * delta;
                if (lerp.Value > 1.0f)
                {
                    lerp.Value -= 1.0f;
                    
                    // Use bitfield to determine next tile
                    // TODO: Check playing field boundaries (probably already done with wall check if boundary has one)
                    // TODO: Add wall check
                    tile.Coords += new int2(new bool2((dir.Value & DirectionEnum.East) != 0, (dir.Value & DirectionEnum.South) != 0));
                    tile.Coords -= new int2(new bool2((dir.Value & DirectionEnum.West) != 0, (dir.Value & DirectionEnum.North) != 0));
                }
            }).ScheduleParallel();
    }
}