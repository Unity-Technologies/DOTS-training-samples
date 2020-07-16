using Unity.Entities;

public class DespawnSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime;

        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();

        Entities.ForEach((int entityInQueryIndex, Entity entity, ref DespawnTimer timer) =>
        {
            timer.Time -= dt;
            
            if (timer.Time <= 0)
                ecb.DestroyEntity(entityInQueryIndex, entity);

        }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
