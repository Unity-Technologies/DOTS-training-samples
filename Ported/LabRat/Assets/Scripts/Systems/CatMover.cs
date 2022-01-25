using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class CatMover : SystemBase
{
    protected override void OnUpdate()
    {
        Config conf = GetSingleton<Config>();
        var delta = Time.DeltaTime;

        Entities
            .WithAll<Cat>()
            .ForEach((ref Tile tile, ref Direction dir, ref TileLerp lerp) =>
            {
                lerp.Value += conf.CatMovementSpeed * delta;
                var direction = new int2((dir.Value & DirectionEnum.East) != 0 ? 1: (dir.Value & DirectionEnum.West) != 0 ? -1 : 0,
                    (dir.Value & DirectionEnum.South) != 0 ? 1 : (dir.Value & DirectionEnum.North) != 0 ? -1 : 0);
                if (lerp.Value > 1.0f)
                {
                    lerp.Value -= 1.0f;
                    
                    // Use bitfield to determine next tile
                    // TODO: Check playing field boundaries (probably already done with wall check if boundary has one)
                    // TODO: Add wall check
                    tile.Coords += direction;
                }
            }).ScheduleParallel();
    }
}