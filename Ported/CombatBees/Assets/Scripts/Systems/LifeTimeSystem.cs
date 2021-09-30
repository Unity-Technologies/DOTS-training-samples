using Unity.Entities;
using Unity.Collections;

public partial class LifeTimeSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var system = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = system.CreateCommandBuffer().AsParallelWriter();

        Entities
            .ForEach((Entity entity, int entityInQueryIndex, ref LifeTime lifetime) =>
            {
                lifetime.TimeRemaining -= deltaTime;

                if (lifetime.TimeRemaining <= 0.0f)
                {
                    ecb.DestroyEntity(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();
        
        system.AddJobHandleForProducer(Dependency);
    }
}