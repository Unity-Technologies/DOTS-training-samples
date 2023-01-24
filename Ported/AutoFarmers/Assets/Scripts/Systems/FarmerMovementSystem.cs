using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

[BurstCompile]
partial struct FarmerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(var (transform, farmer) in SystemAPI.Query<TransformAspect, RefRO<Farmer>>().WithAll<Farmer>())
        {
            float3 diff = farmer.ValueRO.moveTarget - transform.LocalPosition;
            float diffMag = math.length(diff);
            float moveAmount = farmer.ValueRO.moveSpeed * SystemAPI.Time.DeltaTime;
            float moveMin = math.min(moveAmount, diffMag);
            float3 moveDirection = math.normalize(diff) * moveMin;
            if(diffMag > 0.1f)
            {
                transform.LocalPosition += moveDirection;
            }
            else
            {
                transform.LocalPosition = farmer.ValueRO.moveTarget;
            }
        }
    }
}
