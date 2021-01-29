using System;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(TargetingTrackingSystem))]
public class FoodMovementSystem : SystemBase
{
    private EntityCommandBufferSystem ecbs;
    
    protected override void OnCreate()
    {
        ecbs = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var pecb = ecbs.CreateCommandBuffer().AsParallelWriter();
        
        Entities
            .WithName("FoodMovementSystem")
            .WithAll<FoodTag, CarrierBee>()
            .ForEach((Entity e, int entityInQueryIndex, ref PhysicsData physicsData, in CarrierBee b) =>
            {
                if (HasComponent<Translation>(b.Value))
                {
                    var beePosition = GetComponent<Translation>(b.Value);
                    physicsData.v = 0;
                    pecb.SetComponent(entityInQueryIndex, e, new Translation()
                    {
                        Value = beePosition.Value + new float3(0f, -1f, 0f)
                    });
                }
                else
                {
                    //If the bee carrying us doesn't exist, it means it's dead. So we have to drop again. 
                    pecb.RemoveComponent<CarrierBee>(entityInQueryIndex, e);
                }

            }).ScheduleParallel();
    }
}