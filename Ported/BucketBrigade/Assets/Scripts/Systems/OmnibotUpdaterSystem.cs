using System.Collections;
using System.Collections.Generic;
using Enums;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Utilities;

namespace Systems
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    [UpdateAfter(typeof(BucketSpawnerSystem))]
    [UpdateAfter(typeof(BotSpawnerSystem))]
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
            var config = SystemAPI.GetSingleton<ConfigAuthoring.Config>();

            int botCount = 0;
            foreach (var (botTransform, command, botEntity) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRW<BotCommand>>().WithAll<BotOmni>().WithEntityAccess())
            {
                switch (command.ValueRW.Value)
                {
                    case BotAction.GET_BUCKET:
                        var targetBucket = state.EntityManager.GetComponentData<TargetBucket>(botEntity);
                        if (targetBucket.value == Entity.Null)
                        {
                            Debug.Log($"Finding bucket");
                            targetBucket.value = FindBucket(ref state, in botTransform.ValueRW.Position);
                            state.EntityManager.SetComponentData(botEntity, new TargetBucket { value = targetBucket.value });
                        }
                        else
                        {
                            LocalTransform bucketTransform = state.EntityManager.GetComponentData<LocalTransform>(targetBucket.value);

                            if (Utils.MoveTowards(ref botTransform.ValueRW, bucketTransform.Position, config.botSpeed, config.botArriveThreshold))
                            {
                                botCount++;
                                state.EntityManager.SetComponentEnabled<BucketActive>(targetBucket.value, true);
                                // command.ValueRW.Value = BotAction.GOTO_WATER;
                            }
                        
                        }
                        break;
                }
            }
            Debug.Log($"Bot count: {botCount}");
        }
        
        // public void MoveToBucket(ref SystemState state, ref ConfigAuthoring.Config config, ref Entity botEntity)
        // {
        //     foreach (var (transform, command, targetBucket, botEntity)
        //              in SystemAPI.Query<RefRW<WorldTransform>, RefRW<BotCommand>, RefRW<TargetBucket>>()
        //                  .WithEntityAccess())
        //     {
        //         if (command.ValueRO.Value != BotAction.GET_BUCKET) continue;
        //
        //         if (state.EntityManager.IsComponentEnabled<BucketActive>(targetBucket.ValueRO.value) == false)
        //         {
        //             var destination = state.EntityManager.GetComponentData<WorldTransform>(targetBucket.ValueRO.value);
        //             if (MoveTowards(ref transform.ValueRW.Position, destination.Position, config.botSpeed, .01f))
        //             {
        //                 state.EntityManager.SetComponentEnabled<BucketActive>(targetBucket.ValueRO.value, true);
        //                 command.ValueRW.Value = BotAction.GOTO_WATER;
        //             }
        //         }
        //         else
        //         {
        //             targetBucket.ValueRW.value = Entity.Null;
        //         }
        //     }
        // }



        public Entity FindBucket(ref SystemState state, in float3 botPos)
        {
            // foreach (var (transform, command, botEntity)
            //          in SystemAPI.Query<RefRW<WorldTransform>, RefRO<BotCommand>>()
            //              .WithEntityAccess())
            // {
            // if (command.ValueRO.Value != BotAction.GET_BUCKET) continue;

            var minDistance = float.PositiveInfinity;
            var closestBucket = Entity.Null;

            foreach (var (bucket, bucketEntity)
                     in SystemAPI.Query<RefRO<LocalTransform>>()
                         .WithAll<Bucket>()
                         // .WithNone<BucketActive, BucketFull>()
                         .WithEntityAccess())
            {
                var distance = math.distancesq(botPos, bucket.ValueRO.Position);

                if (distance < minDistance)
                {
                    closestBucket = bucketEntity;
                    minDistance = distance;
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
}