using Unity.Entities;

[DisableAutoCreation]
[UpdateBefore(typeof(CooldownSystem))]
public class CooldownTestSystem : SystemBase
{
    private EntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate()
    {
        _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer.Concurrent ecb = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        Entities
            .WithNone<Cooldown>()
            .ForEach((int entityInQueryIndex, Entity entity) =>
            {
                ecb.AddComponent(entityInQueryIndex, entity, new Cooldown() { Value = 1f});
            }).ScheduleParallel();

        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency); 
    }
}