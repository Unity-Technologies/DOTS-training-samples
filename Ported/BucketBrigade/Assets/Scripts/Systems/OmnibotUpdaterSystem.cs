using System.Collections;
using System.Collections.Generic;
using Enums;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct OmnibotUpdaterSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigAuthoring.Config>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (transform, command, botEntity) in
                 SystemAPI.Query<RefRW<WorldTransform>, RefRW<BotCommand>>().WithAll<BotOmni>().WithEntityAccess())
        {
            switch (command.ValueRW.Value)
            {
                case BotAction.GET_BUCKET:
                    Entity bucket = FindBucket(ref state, in transform.ValueRW.Position);
                    if(bucket != Entity.Null)
                    {
                        state.EntityManager.SetComponentData(botEntity, new TargetBucket { value = bucket });
                    }
                    MoveToBucket(ref state);
                    break;
            }
        }
    }
    public void MoveToBucket(ref SystemState state)
    {
        foreach (var (transform, botEntity)
                 in SystemAPI.Query<RefRW<WorldTransform>>()
                     .WithEntityAccess())
        {

        }
    }
    
    public Entity FindBucket(ref SystemState state, in float3 pos)
    {
        // foreach (var (transform, command, botEntity)
        //          in SystemAPI.Query<RefRW<WorldTransform>, RefRO<BotCommand>>()
        //              .WithEntityAccess())
        // {
            // if (command.ValueRO.Value != BotAction.GET_BUCKET) continue;

            var minDistance = float.PositiveInfinity;
            var minPosition = float3.zero;
            var closestBucket = Entity.Null;

            foreach (var (bucket, entity)
                     in SystemAPI.Query<RefRO<WorldTransform>>()
                         .WithAll<Bucket>()
                         .WithNone<BucketActive, BucketFull>()
                         .WithEntityAccess())
            {
                var distance = math.distancesq(pos, bucket.ValueRO.Position);

                if (distance < minDistance)
                {
                    closestBucket = entity;
                    minDistance = distance;
                    minPosition = bucket.ValueRO.Position;
                }
            }

            if (closestBucket != Entity.Null)
            {
                // state.EntityManager.SetComponentData(botEntity, new TargetBucket { value = closestBucket });
                Debug.Log($"Closest distance {minDistance}");
            }

            return closestBucket;

            // }
    }
}
