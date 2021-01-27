using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;


// Any type inheriting from SystemBase will be registered as a system and will start
// updating every frame.
[UpdateBefore(typeof(TargetAcquisitionSystem))]
[UpdateBefore(typeof(TargetingValidationSystem))]
public class PickupSystem : SystemBase
{
    private EntityCommandBufferSystem CommandBufferSystem;
    
    protected override void OnCreate()
    {
        CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.
        Entities
            .WithName("Pickup")
            .WithAll<BeeTag>()
            .WithNone<CarriedFood>()
            .ForEach((Entity e, int entityInQueryIndex, ref Translation translation, ref TargetPosition t, in MoveTarget moveTarget) =>
            {
                if (MathUtil.IsWithinDistance(1.0f, t.Value, translation.Value))
                {
                    ecb.AddComponent(entityInQueryIndex, e, new CarriedFood() {Value = moveTarget.Value});
                    ecb.AddComponent(entityInQueryIndex, moveTarget.Value, new CarrierBee() {Value = e});
                    //ecb.RemoveComponent<PhysicsData>(entityInQueryIndex, moveTarget.Value);
                    ecb.RemoveComponent<MoveTarget>(entityInQueryIndex, e);
                    t.Value = float3.zero;
                }
                
            }).ScheduleParallel();
            
        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}