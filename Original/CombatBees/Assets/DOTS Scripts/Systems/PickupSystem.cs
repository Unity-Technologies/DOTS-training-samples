using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

// Any type inheriting from SystemBase will be registered as a system and will start
// updating every frame.
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
            .WithoutBurst()
            .WithAll<BeeTag>()
            .WithNone<CarriedResource>()
            .ForEach((Entity e, int entityInQueryIndex, ref Translation translation, ref TargetPosition t, in MoveTarget moveTarget) =>
            {
                if (MathUtil.IsWithinDistance(1.0f, t.Value, translation.Value))
                {
                    ecb.AddComponent(entityInQueryIndex, e, new CarriedResource() {Value = moveTarget.Value});
                    ecb.AddComponent(entityInQueryIndex, moveTarget.Value, new CarrierBee() {Value = e});
                    ecb.RemoveComponent<MoveTarget>(entityInQueryIndex, e);
                    t.Value = float3.zero;
                }
                
            }).ScheduleParallel();
            
        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}