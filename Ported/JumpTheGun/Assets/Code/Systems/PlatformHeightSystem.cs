
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class PlatformHeightSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        float2 board = GetSingleton<MinMaxHeight>().Value;
        float hitStrength = GetSingleton<HitStrength>().Value;

        Entities.
            WithAll<Platform, WasHit>()
            .ForEach((ref LocalToWorld xform, ref URPMaterialPropertyBaseColor baseColor, ref WasHit hit) =>
            {
                float posY = math.max(board.x, xform.Position.y - hit.Count * hitStrength);

                baseColor.Value = Colorize.Platform(posY, board.x, board.y);
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
