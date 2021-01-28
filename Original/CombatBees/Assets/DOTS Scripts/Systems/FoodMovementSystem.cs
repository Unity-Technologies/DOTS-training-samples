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

        Entities
            .WithName("CarriedFoodMovementSystem")
            .WithNativeDisableParallelForRestriction(allTranslations)
            .WithAll<FoodTag, CarrierBee>()
            .ForEach((Entity e, ref PhysicsData physicsData, in CarrierBee b) =>
            {
                var beePosition = allTranslations[b.Value];
                var foodPos = allTranslations[e];
                foodPos.Value = beePosition.Value + new float3(0f, -1f, 0f);
                physicsData.v = 0;
                allTranslations[e] = foodPos;

            }).ScheduleParallel();
    }
}