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
        var allTranslations = GetComponentDataFromEntity<Translation>();
        var deltaTime = Time.DeltaTime;
        var gravity = new float3(0f, -30f, 0f);

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
            .ForEach((Entity e, ref Translation t, ref PhysicsData physicsData) =>
            {
                physicsData.a += gravity;
            }).ScheduleParallel(); //TODO: Change to schedule parallel (EntityQueryIndex)
    }
}