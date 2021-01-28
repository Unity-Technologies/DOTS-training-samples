using System;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(TargetingTrackingSystem))]
public class GravitySystem : SystemBase
{
    protected override void OnUpdate()
    {
        var gravity = new float3(0f, -30f, 0f);

        Entities
            .WithName("GravitySystem")
            .WithAny<FoodTag, BeeCorpseTag>()
            .WithNone<CarrierBee>()
            .ForEach((Entity e, ref Translation t, ref PhysicsData physicsData) =>
            {
                physicsData.a += gravity;
            }).ScheduleParallel();
    }
}