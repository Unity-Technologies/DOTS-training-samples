using Unity.Entities;

[UpdateAfter(typeof(ClickSpawnSystem))]
public class TargetingValidationSystem : SystemBase
{    
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);//.CreateCommandBuffer();//.AsParallelWriter();
        var carriedFood = GetComponentDataFromEntity<CarrierBee>(true);
        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.
        Entities
            .WithName("TargetValidation")
            .WithoutBurst()
            .WithReadOnly(carriedFood)
            .WithAll<MoveTarget>()
            .ForEach((Entity e, ref MoveTarget targetFood) => 
            {
                Entity targetEntity = targetFood.Value;
                
                if(!EntityManager.Exists(targetEntity) || // if previously destroyed
                    carriedFood.HasComponent(targetEntity)) // if carried by another bee
                {
                    ecb.RemoveComponent<MoveTarget>( e);
                    ecb.RemoveComponent<TargetPosition>( e);
                    ecb.RemoveComponent<CarriedFood>( e);
                }
            }).Run();
        
        ecb.Playback(EntityManager);

        ecb.Dispose();
    }
}