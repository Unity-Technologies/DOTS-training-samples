using Unity.Entities;
using Unity.Rendering;

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
 
        var ecb = EntityCommandBufferSystem.CreateCommandBuffer();

        Entities
            .ForEach((Entity entity, ref Target target) =>
            {
                var targetEntity = target.Value;
                
                if (HasComponent<LifeSpan>(targetEntity))
                {
                    ecb.RemoveComponent<Target>(entity);
                }
            }).Schedule();


        Entities
            .ForEach((Entity entity, ref LifeSpan lifespan) =>
            {
                lifespan.Value -= timeDeltaTime;

                if (lifespan.Value <= 0)
                {
                    ecb.DestroyEntity(entity);
                }
            }).Schedule();

        EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
