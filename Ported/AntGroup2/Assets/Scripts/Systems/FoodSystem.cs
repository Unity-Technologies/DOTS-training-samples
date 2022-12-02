using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Rendering;

[UpdateBefore(typeof(AntMovementSystem))]
[UpdateAfter(typeof(WorldSpawnerSystem))]
partial struct FoodSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Food>();
    }


    public void OnDestroy(ref SystemState state)
    {
    }


    public void OnUpdate(ref SystemState state)
    {
        var colonyLocation = SystemAPI.GetComponent<LocalTransform>(SystemAPI.GetSingletonEntity<Colony>()).Position;
        var foodLocation = SystemAPI.GetComponent<LocalTransform>(SystemAPI.GetSingletonEntity<Food>()).Position;

        var job = new FoodJob { colonyLocation = colonyLocation, foodLocation = foodLocation };
        job.ScheduleParallel();
    }

    [BurstCompile]
    [WithAll(typeof(Ant))]
    partial struct FoodJob : IJobEntity
    {
        public float3 colonyLocation;
        public float3 foodLocation;

        public void Execute(in Position position, ref HasResource hasResource, ref URPMaterialPropertyBaseColor color)
        {
            bool currentlyHasResource = hasResource.Value;
            float3 targetPosition = currentlyHasResource ? colonyLocation : foodLocation;
            if (math.distance(position.Value, targetPosition.xz) < 1.0f) // TODO Hard coded food radius of 1 m
            {
                hasResource.Value = !currentlyHasResource;
                hasResource.Trigger = true;
                color.Value = currentlyHasResource ? new float4(1.0f, 1.0f, 1.0f, 1.0f) : new float4(0.0f, 1.0f, 1.0f, 1.0f);
            }
            else
            {
                hasResource.Trigger = false;
            }
        }
    }
}
