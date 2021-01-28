using Unity.Collections;
using Unity.Entities;

[UpdateAfter(typeof(ClickSpawnSystem))]
public class TargetingValidationSystem : SystemBase
{    
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var pEcb = ecb.AsParallelWriter();
        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.
        Entities
            .WithName("TargetValidation")
            .WithAll<MoveTarget>()
            .ForEach((Entity e, int entityInQueryIndex, ref MoveTarget targetFood) => 
            {
                Entity targetEntity = targetFood.Value;
                
                if(!HasComponent<FoodTag>(targetEntity) || // if previously destroyed
                    HasComponent<CarrierBee>(targetEntity)) // if carried by another bee
                {
                    pEcb.AddComponent<BeeWithInvalidtarget>(entityInQueryIndex, e);
                }
            }).ScheduleParallel();
        
        Dependency.Complete();
        ecb.Playback(EntityManager);

        ecb.Dispose();
    }
}