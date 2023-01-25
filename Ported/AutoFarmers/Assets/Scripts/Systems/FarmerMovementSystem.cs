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
        foreach(var farmer in SystemAPI.Query<FarmerAspect>())
        {
            float3 diff = farmer.MoveTarget - farmer.Transform.WorldPosition;
            float diffMag = math.length(diff);
            float moveAmount = farmer.MoveSpeed * SystemAPI.Time.DeltaTime;
            float moveMin = math.min(moveAmount, diffMag);
            float3 moveDirection = math.normalize(diff) * moveMin;
            moveDirection.y = 0;
            if(diffMag > 0.1f)
            {
                farmer.Transform.WorldPosition += moveDirection;
            }
            else
            {
                farmer.Transform.WorldPosition = farmer.MoveTarget;
            }
        }
    }
}
