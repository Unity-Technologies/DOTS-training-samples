using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class CooldownSystem : SystemBase
{
    private EntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate()
    {
        _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float delta = Time.DeltaTime;
        EntityCommandBuffer.Concurrent ecb = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        Entities
            .ForEach((int entityInQueryIndex, Entity entity, ref Cooldown cooldown) =>
            {
                cooldown.Value -= delta;

                if (cooldown.Value <= 0f)
                {
                    ecb.RemoveComponent<Cooldown>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();

        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency); 
    }
}