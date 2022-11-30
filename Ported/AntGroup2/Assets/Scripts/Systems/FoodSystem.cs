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
        //var config = SystemAPI.GetSingleton<Config>();

        var colonyLocation = SystemAPI.GetComponent<LocalTransform>(SystemAPI.GetSingletonEntity<Colony>()).Position;
        var foodLocation = SystemAPI.GetComponent<LocalTransform>(SystemAPI.GetSingletonEntity<Food>()).Position;

        //foreach (var foodTransform in SystemAPI.Query<TransformAspect>().WithAll<Food>())
        {
            foreach (var (transform, hasResource, color) in SystemAPI.Query<TransformAspect, RefRW<HasResource>, RefRW<URPMaterialPropertyBaseColor>>().WithAll<Ant>())
            {
                bool currentlyHasResource = hasResource.ValueRO.Value;
                float3 targetPosition = currentlyHasResource ? colonyLocation : foodLocation;
                if (math.distance(transform.LocalPosition, targetPosition) < 1.0f) // TODO Hard coded food radius of 1 m
                {
                    hasResource.ValueRW.Value = !currentlyHasResource;
                    hasResource.ValueRW.Trigger = true;
                    color.ValueRW.Value = currentlyHasResource ? new float4(1.0f, 1.0f, 1.0f, 1.0f) : new float4(0.0f, 1.0f, 1.0f, 1.0f);
                }
                else
                {
                    hasResource.ValueRW.Trigger = false;
                }
            }
        }
    }
}
