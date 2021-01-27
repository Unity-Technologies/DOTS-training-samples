using System;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(TargetingTrackingSystem))]
public class FoodMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var allTranslations = GetComponentDataFromEntity<Translation>();
        var deltaTime = Time.DeltaTime;
        var gravity = new float3(0f, -30f, 0f);

        Entities
            .WithName("CarriedFoodMovementSystem")
            //.WithNativeDisableParallelForRestriction(allTranslations)
            .WithAll<FoodTag, CarrierBee>()
            .ForEach((Entity e, in CarrierBee b) =>
            {
                var beePosition = allTranslations[b.Value];
                var foodPos = allTranslations[e];
                foodPos.Value = beePosition.Value + new float3(0f, -0.5f, 0f);
                allTranslations[e] = foodPos;

            }).Run();
        
        Entities
            .WithName("FreeFoodMovementSystem")
            .WithAll<FoodTag>()
            .WithNone<CarrierBee>()
            .ForEach((Entity e, ref Translation t, ref PhysicsData physicsData) =>
            {
                physicsData.a += gravity;
            }).Run();
    }
}