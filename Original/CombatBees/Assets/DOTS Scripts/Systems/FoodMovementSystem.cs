using System;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

// Any type inheriting from SystemBase will be registered as a system and will start
// updating every frame.
public class FoodMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var allTranslations = GetComponentDataFromEntity<Translation>(false);
        var deltaTime = Time.DeltaTime;
        var fall = new float3(0f, -1f, 0f) * deltaTime;

        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.
        Entities
            .WithName("CarriedFoodMovementSystem")
            .WithNativeDisableParallelForRestriction(allTranslations)
            .WithAll<FoodTag, CarrierBee>()
            .ForEach((Entity e, in CarrierBee b) =>
            {
                var beePosition = allTranslations[b.Value];
                var foodPos = allTranslations[e];
                foodPos.Value = beePosition.Value + new float3(0f, -0.5f, 0f);
                allTranslations[e] = foodPos;

            }).ScheduleParallel(); //TODO: Change to schedule parallel (EntityQueryIndex)
        
        Entities
            .WithName("FreeFoodMovementSystem")
            .WithAll<FoodTag>()
            .WithNone<CarrierBee>()
            .ForEach((Entity e, ref Translation t) =>
            {
                if (t.Value.y > 0)
                {
                    t.Value += fall;
                    if (t.Value.y < 0)
                        t.Value.y = 0;
                }
                
            }).ScheduleParallel(); //TODO: Change to schedule parallel (EntityQueryIndex)
    }
}