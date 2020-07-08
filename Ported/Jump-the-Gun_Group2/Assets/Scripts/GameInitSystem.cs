using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class GameInitSystem : SystemBase
{
    EntityCommandBufferSystem m_ECBSystem;
    EntityQuery m_Query;

    protected override void OnCreate()
    {
        m_Query = GetEntityQuery(new ComponentType(typeof(GameOverTag)));
        RequireForUpdate(m_Query);

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer();

        Entities
            .ForEach((int entityInQueryIndex, Entity entity, in GameParams gameParams) =>
        {
            for (int y = 0; y < gameParams.TerrainDimensions.y; ++y)
            {
                for (int x = 0; x < gameParams.TerrainDimensions.x; ++x)
                {
                    var instance = ecb.Instantiate(gameParams.TilePrefab);
                    ecb.SetComponent(instance, new Position { Value = new float3(x, 0, y) });
                }
            }
            ecb.RemoveComponent<GameOverTag>(entity);
        }).Schedule();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}