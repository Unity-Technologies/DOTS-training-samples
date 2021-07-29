
using Unity.Entities;

namespace DOTSRATS
{
    [UpdateAfter(typeof(Movement))]
    public class DeathSystem : SystemBase
    {
        EntityCommandBufferSystem CommandBufferSystem;

        protected override void OnCreate()
        {
            CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            var deltaTime = Time.DeltaTime;

            Entities
                .WithAll<Death>()
                .ForEach((Entity entity, int entityInQueryIndex, ref Death death) =>
                {
                    death.deathTimer -= deltaTime;
                    if(death.deathTimer <= 0)
                    {
                        ecb.DestroyEntity(entityInQueryIndex, entity);
                    }
                }).ScheduleParallel();

            CommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
