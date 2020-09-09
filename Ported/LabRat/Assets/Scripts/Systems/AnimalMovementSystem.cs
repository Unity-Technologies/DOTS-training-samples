using Unity.Entities;


public class AnimalMovementSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();

    }

    protected override void OnUpdate()
    {
        var time = Time.DeltaTime;
        
        var ecb = m_CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        
        Entities
            .WithNone<Falling>()
            .ForEach((int entityInQueryIndex, Entity spawnerEntity, ref PositionXZ pos, in Speed speed) =>
        {
            pos.Value.x += speed.Value * time;
            
            if (pos.Value.x >= 32)
            {
                ecb.AddComponent<Falling>(entityInQueryIndex, spawnerEntity);
                //TODO(ddebaets) stop moving in xz plane, set direction to 0 ?
            }
        }).ScheduleParallel();
        
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);

    }
}
