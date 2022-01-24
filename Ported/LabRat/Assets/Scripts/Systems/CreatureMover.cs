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
                    tile.Value += new int2(new bool2((dir.Value & Dir.East) != 0, (dir.Value & Dir.South) != 0));
                    tile.Value -= new int2(new bool2((dir.Value & Dir.West) != 0, (dir.Value & Dir.North) != 0));
                }
            }).ScheduleParallel();
    }
}