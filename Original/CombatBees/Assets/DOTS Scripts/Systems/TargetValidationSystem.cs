using Unity.Entities;

// Any type inheriting from SystemBase will be registered as a system and will start
// updating every frame.
[UpdateBefore(typeof(BeeMoveToTargetSystem))]
public class TargetingValidationSystem : SystemBase
{
    private EntityCommandBufferSystem CommandBufferSystem;
    
    protected override void OnCreate()
    {
        CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        var ecb = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        var carriedFood = GetComponentDataFromEntity<CarrierBee>(true);
        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.
        Entities
            .WithName("TargetValidation")
            .WithoutBurst()
            .WithReadOnly(carriedFood)
            .WithAll<MoveTarget>()
            .ForEach((Entity e, int entityInQueryIndex, ref MoveTarget targetFood) => {
                if (carriedFood.HasComponent(targetFood.Value))
                {
                    ecb.RemoveComponent<MoveTarget>(entityInQueryIndex, e);
                    ecb.RemoveComponent<TargetPosition>(entityInQueryIndex, e);
                    ecb.RemoveComponent<CarriedFood>(entityInQueryIndex, e);
                }
            }).ScheduleParallel();
        
        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}