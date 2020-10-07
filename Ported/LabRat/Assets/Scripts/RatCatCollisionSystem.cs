using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class RatCatCollisionSystem : SystemBase
{
    private const float CollisionRadiusSq = 0.04f;

    private EntityCommandBufferSystem m_ECBSystem;
    private EntityQuery m_CatPositionsQuery;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        EntityQueryDesc desc = new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(Cat), typeof(Translation)}
        };
        m_CatPositionsQuery = EntityManager.CreateEntityQuery(desc);
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
        var catTranslations = m_CatPositionsQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        Entities.WithAll<Rat>().ForEach(
            (Entity entity, int entityInQueryIndex, ref Position position, ref Translation translation) =>
            {
                foreach (var catTranslation in catTranslations)
                {
                    if (math.distancesq(translation.Value, catTranslation.Value) < CollisionRadiusSq)
                    {
                        ecb.DestroyEntity(entityInQueryIndex, entity);
                    }
                }
            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
        catTranslations.Dispose(Dependency);
    }
}