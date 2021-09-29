using Unity.Entities;
using Unity.Collections;

public partial class DecaySystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var system = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = system.CreateCommandBuffer().AsParallelWriter();

        Entities
            .ForEach((Entity entity, int entityInQueryIndex, ref Particle particle) =>
            {
                particle.LifeRemaining -= deltaTime;

                if (particle.LifeRemaining <= 0.0f)
                {
                    ecb.DestroyEntity(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();
        
        system.AddJobHandleForProducer(Dependency);
    }
}