using Unity.Entities;

[UpdateAfter(typeof(BeeUpdateGroup))]
//[UpdateAfter(typeof(BeeReturningSystem))]
public class DestroyerSystem : SystemBase
{
    private EntityCommandBufferSystem EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var timeDeltaTime = Time.DeltaTime;
 
        var ecb = EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithName("RemoveDeadTarget")
            .ForEach((int entityInQueryIndex, Entity entity, ref Target target) =>
            {
                var targetEntity = target.Value;
                
                if (HasComponent<LifeSpan>(targetEntity))
                {
                    ecb.RemoveComponent<Target>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();
        
        Entities
            .WithName("DestroyEntity")
            .ForEach((int entityInQueryIndex, Entity entity, ref LifeSpan lifespan) =>
            {
                lifespan.Value -= timeDeltaTime;

                if (lifespan.Value <= 0)
                {
                    ecb.DestroyEntity(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();

        EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
