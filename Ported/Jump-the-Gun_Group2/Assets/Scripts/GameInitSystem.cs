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
        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();

        Entities
            .ForEach((int entityInQueryIndex, Entity entity, in GameParams gameParams) =>
        {
            //var brickSize = GetComponent<Scale2D>(spawner.BrickPrefab).Value;
            for (int y = 0; y < gameParams.TerrainDimensions.y; ++y)
            {
                for (int x = 0; x < gameParams.TerrainDimensions.x; ++x)
                {
                    var instance = ecb.Instantiate(entityInQueryIndex, gameParams.TilePrefab);
              //      var translation = spawnerTranslation2D.Value + new float2(x - (spawner.SizeX - 1) / 2f, y);
              //      translation *= brickSize * 1.1f;
              //      ecb.SetComponent(instance, new Translation2D { Value = translation });
                }
            }
            //ecb.AddComponent<DisabledTag>(spawnerEntity);
        }).Schedule();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}