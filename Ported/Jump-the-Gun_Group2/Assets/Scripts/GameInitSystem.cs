using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class GameInitSystem : SystemBase
{
    EntityCommandBufferSystem m_ECBSystem;
    EntityQuery m_Query;
    Random m_Random;

    protected override void OnCreate()
    {
        m_Query = GetEntityQuery(new ComponentType(typeof(GameOverTag)));
        RequireForUpdate(m_Query);

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        m_Random = new Random(0x1234567);
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer();
        var random = m_Random;

        Entities
            .ForEach((int entityInQueryIndex, Entity entity, in GameParams gameParams) =>
        {
            for (int y = 0; y < gameParams.TerrainDimensions.y; ++y)
            {
                for (int x = 0; x < gameParams.TerrainDimensions.x; ++x)
                {
                    var instance = ecb.Instantiate(gameParams.TilePrefab);
                    var height = gameParams.TerrainHeightRange.x + random.NextFloat() * (gameParams.TerrainHeightRange.y - gameParams.TerrainHeightRange.x);
                    ecb.SetComponent(instance, new Position { Value = new float3(x, 0, y) });
                    ecb.SetComponent(instance, new Height { Value = height });
                }
            }
            ecb.RemoveComponent<GameOverTag>(entity);
        }).Schedule();

        m_Random = random;

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}