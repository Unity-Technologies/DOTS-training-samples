using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

[UpdateBefore(typeof(AntMovementSystem))]
partial struct FoodSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }


    public void OnDestroy(ref SystemState state)
    {
    }


    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        var colonyLocation = SystemAPI.GetComponent<LocalTransform>(SystemAPI.GetSingletonEntity<Colony>()).Position;
        var foodLocation = SystemAPI.GetComponent<LocalTransform>(SystemAPI.GetSingletonEntity<Food>()).Position;

        //foreach (var foodTransform in SystemAPI.Query<TransformAspect>().WithAll<Food>())
        {
            foreach (var (transform, hasResource) in SystemAPI.Query<TransformAspect, RefRW<HasResource>>().WithAll<Ant>())
            {
                bool currentlyHasResource = hasResource.ValueRO.Value;
                float3 targetPosition = currentlyHasResource ? colonyLocation : foodLocation;
                if (math.distance(transform.LocalPosition, targetPosition) < 1.0f) // TODO Hard coded food radius of 1 m
                {
                    hasResource.ValueRW.Value = !currentlyHasResource;
                    hasResource.ValueRW.Trigger = true;
                }
                else
                {
                    hasResource.ValueRW.Trigger = false;
                }
            }
        }
    }
}
