
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateInGroup(typeof(Unity.Entities.SimulationSystemGroup))]
public class PlatformHeightSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (TryGetSingleton<IsPaused>(out _))
            return;

        float2 board = GetSingleton<MinMaxHeight>().Value;
        float hitStrength = GetSingleton<HitStrength>().Value;

        Entities
            .WithDisposeOnCompletion(board)
            .WithDisposeOnCompletion(hitStrength)
            .WithAll<Platform, WasHit>()
            .ForEach((int entityInQueryIndex, Entity entity, ref LocalToWorld xform, ref URPMaterialPropertyBaseColor baseColor, ref WasHit hit) =>
            {
                if (hit.Count <= 0)
                {
                    return;
                }

                float posY = math.max(board.x, xform.Position.y - hit.Count * hitStrength);

                baseColor.Value = Colorize.Platform(posY, board.x, board.y);
            })
            .Run();
    }
}
