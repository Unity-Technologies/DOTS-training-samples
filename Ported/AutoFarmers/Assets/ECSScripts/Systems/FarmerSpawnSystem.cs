using Unity.Entities;

public class FarmerSpawnSystem : SystemBase
{
    EntityCommandBufferSystem m_ECBSystem;
    
    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();    
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        var gameStateEntity = GetSingletonEntity<GameState>();
        var gameState = GetComponent<GameState>(gameStateEntity);
        
        Entities
            .WithAll<Depot>()
            .ForEach((
                    Entity entity,
                    int entityInQueryIndex,
                    ref DepotCanSpawn canSpawn,
                    in Position position) =>
                {
                    var farmerEntity = ecb.Instantiate(entityInQueryIndex, gameState.FarmerPrefab);
                    ecb.SetComponent(entityInQueryIndex, farmerEntity, new Position(){Value = position.Value});
                    ecb.RemoveComponent<DepotCanSpawn>(entityInQueryIndex, entity);   
                }).ScheduleParallel();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
