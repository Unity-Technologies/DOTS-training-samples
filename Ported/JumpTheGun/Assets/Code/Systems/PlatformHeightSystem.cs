
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class PlatformHeightSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        var query = GetEntityQuery(typeof(Platform), typeof(WasHit));
        RequireForUpdate(query);

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        if (TryGetSingleton<IsPaused>(out _))
            return;

        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        float2 board = GetSingleton<MinMaxHeight>().Value;
        float hitStrength = GetSingleton<HitStrength>().Value;

        Entities.
            WithAll<Platform, WasHit>()
            .ForEach((int entityInQueryIndex, Entity entity, ref LocalToWorld xform, ref URPMaterialPropertyBaseColor baseColor, ref WasHit hit, ref TargetPosition targetPosition) =>
            {
                if (hit.Count <= 0)
                {
                    ecb.RemoveComponent<WasHit>(entityInQueryIndex, entity);
                    return;
                }

                float posY = math.max(board.x, xform.Position.y - hit.Count * hitStrength);

                targetPosition.Value.y = posY;

                baseColor.Value = Colorize.Platform(posY, board.x, board.y);

                ecb.RemoveComponent<WasHit>(entityInQueryIndex, entity);
            })
            .WithDisposeOnCompletion(board)
            .WithDisposeOnCompletion(hitStrength)
            .ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
