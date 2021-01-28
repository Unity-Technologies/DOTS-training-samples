using Unity.Collections;
using Unity.Entities;

[UpdateAfter(typeof(ClickSpawnSystem))]
public class TargetingValidationSystem : SystemBase
{    
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);//.CreateCommandBuffer();//.AsParallelWriter();
        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.
        Entities
            .WithName("TargetValidation")
            .WithAll<MoveTarget>()
            .ForEach((Entity e, ref MoveTarget targetFood) => 
            {
                Entity targetEntity = targetFood.Value;
                
                if(!HasComponent<FoodTag>(targetEntity) || // if previously destroyed
                    HasComponent<CarrierBee>(targetEntity)) // if carried by another bee
                {
                    ecb.RemoveComponent<MoveTarget>( e);
                    ecb.RemoveComponent<TargetPosition>( e);
                    ecb.RemoveComponent<CarriedFood>( e);
                    
                    ecb.RemoveComponent<FetchingFodTag>( e);
                    ecb.RemoveComponent<AttackingBeeTag>( e);
                }
            }).Run();
        
        Dependency.Complete();
        ecb.Playback(EntityManager);

        ecb.Dispose();
    }
}