using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(ArrowPlacerSystem))]
public partial class AICursorSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var config = GetSingleton<Config>();
        var miceQuery = GetEntityQuery(ComponentType.ReadOnly<Mouse>(), ComponentType.ReadOnly<Tile>());
        var tiles = miceQuery.ToComponentDataArray<Tile>(Allocator.TempJob);
        var delta = Time.DeltaTime;

        Entities.ForEach((ref Translation translation, ref CursorPosition position, ref CursorLerp cursor, ref PlayerSpawnArrow shouldSpawnArrow) =>
        {
            if (cursor.LerpValue >= 1f)
            {
                position.Value = cursor.Destination;
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
                var x = math.lerp(position.Value.x, cursor.Destination.x, cursor.LerpValue);
                var y = math.lerp(position.Value.y, cursor.Destination.y, cursor.LerpValue);
                translation.Value += new float3(x, y, 0);
            }

        }).Schedule();
    }
}
