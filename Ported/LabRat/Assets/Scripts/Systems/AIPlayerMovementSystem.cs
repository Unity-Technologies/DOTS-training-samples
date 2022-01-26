using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(ArrowPlacerSystem))]
public partial class AIPlayerMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var config = GetSingleton<Config>();
        var miceQuery = GetEntityQuery(ComponentType.ReadOnly<Mouse>(), ComponentType.ReadOnly<Tile>());
        var tiles = miceQuery.ToComponentDataArray<Tile>(Allocator.TempJob);
        var delta = Time.DeltaTime;

        Entities.ForEach((ref CursorPosition position, ref CursorLerp cursor, ref PlayerSpawnArrow shouldSpawnArrow) =>
        {
            if (cursor.LerpValue >= 1f)
            { 
                cursor.Start = cursor.Destination;
                shouldSpawnArrow.Value = true;

                if (tiles.Length > 0 && config.CursorAIRandom.NextFloat(0f, 1f) > 0.2f)
                {
                    cursor.Destination = tiles[config.CursorAIRandom.NextInt(tiles.Length)].Coords
                                            + config.CursorAIRandom.NextFloat2(new float2(0.01f, 0.01f), new float2(0.99f,0.99f));
                }
                else
                {

                    cursor.Destination = config.CursorAIRandom.NextFloat2(new float2(0f, 0f), new float2(config.MapWidth, config.MapHeight));
                }
                cursor.LerpValue = 0f;
            }
            else
            {
                cursor.LerpValue += config.CursorSpeed * delta;
                var x = math.lerp(cursor.Start.x, cursor.Destination.x, cursor.LerpValue);
                var y = math.lerp(cursor.Start.y, cursor.Destination.y, cursor.LerpValue);
                position.Value = new float2(x, y);
            }

        }).Run();
        tiles.Dispose();
    }
}
