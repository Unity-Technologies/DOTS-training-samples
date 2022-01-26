using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(ArrowPlacerSystem))]
public partial class AIPlayerMovementSystem : SystemBase
{
    private EntityQuery miceQuery;

    protected override void OnCreate()
    {
        miceQuery = GetEntityQuery(ComponentType.ReadOnly<Mouse>(), ComponentType.ReadOnly<Tile>());
        RequireSingletonForUpdate<GameRunning>();
    }

    protected override void OnUpdate()
    {
        var config = GetSingleton<Config>();
        var tiles = miceQuery.ToComponentDataArray<Tile>(Allocator.TempJob);
        var delta = Time.DeltaTime;

        Entities
            .WithReadOnly(tiles)
            .WithDisposeOnCompletion(tiles)
            .ForEach((ref PlayerAIControlled aiState, ref CursorPosition position, ref CursorLerp cursor, ref PlayerSpawnArrow shouldSpawnArrow) =>
        {
            if (cursor.LerpValue >= 1f)
            { 
                cursor.Start = cursor.Destination;
                shouldSpawnArrow.Value = true;

                if (tiles.Length > 0 && aiState.Random.NextFloat(0f, 1f) > 0.2f)
                {
                    cursor.Destination = tiles[aiState.Random.NextInt(tiles.Length)].Coords
                                            + aiState.Random.NextFloat2(new float2(0.01f, 0.01f), new float2(0.99f,0.99f));
                }
                else
                {

                    cursor.Destination = aiState.Random.NextFloat2(new float2(0f, 0f), new float2(config.MapWidth, config.MapHeight));
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

        }).Schedule();
    }
}
