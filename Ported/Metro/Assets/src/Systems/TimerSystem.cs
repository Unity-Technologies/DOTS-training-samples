using Unity.Entities;
using Unity.Jobs;

public class TimerSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithName("timer")
            .ForEach((Entity entity, int entityInQueryIndex, ref WaitTimer timer) =>
            {
                timer.Value -= deltaTime;
                if (timer.Value <= 0.0f)
                {
                    ecb.RemoveComponent<WaitTimer>(entityInQueryIndex, entity);
                }

            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
