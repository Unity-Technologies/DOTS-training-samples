using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(BeeUpdateGroup))]
[UpdateAfter(typeof(BeePerception))]
public class BeeReturningSystem : SystemBase
{
    private EntityCommandBufferSystem EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        // Query for bees that are close enough to a Resource target to collect the Resource
        // TODO how to stop 2 bees collecting the same Resource

        var offset = new float3(0, -0.98f, 0);

        Entities
             .WithName("ReturnResource")
             .WithAll<IsReturning>()
             .ForEach((int entityInQueryIndex, Entity entity, in TargetPosition targetPosition, in Target target, in Translation translation) =>
             {
                 var targetEntity = target.Value;

                 if (HasComponent<IsCarried>(targetEntity))
                 {
                     // if bee is close enough to Base
                     if (math.distancesq(translation.Value, targetPosition.Value) < 0.1)
                     {
                         ecb.RemoveComponent<IsReturning>(entityInQueryIndex, entity);
                         ecb.RemoveComponent<Target>(entityInQueryIndex, entity);
                         ecb.RemoveComponent<IsCarried>(entityInQueryIndex, targetEntity);
                         ecb.AddComponent<HasGravity>(entityInQueryIndex, targetEntity);
                     }
                     else
                     {
                         ecb.SetComponent(entityInQueryIndex, targetEntity, new Translation
                         {
                             Value = translation.Value + offset
                         });
                     }
                 }
             }).ScheduleParallel();

        EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
