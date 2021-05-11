
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

        Board board = GetSingleton<Board>();

        Entities.
            WithAll<Platform, WasHit>()
            .ForEach((ref LocalToWorld xform, ref URPMaterialPropertyBaseColor baseColor, ref WasHit hit) =>
            {
                float posY = math.max(board.MinHeight, xform.Position.y - hit.Count * board.HitStrength);

                baseColor.Value = Colorize.Platform(posY, board.MinHeight, board.MaxHeight);
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
