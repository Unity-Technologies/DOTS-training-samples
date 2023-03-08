using Enums;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI.Table;

namespace Systems
{
    public partial struct GetBucketSystem : ISystem
    {
        // [BurstCompile]
        // public void OnCreate(ref SystemState state)
        // {
        //     state.RequireForUpdate<ConfigAuthoring.Config>();
        // }
        //
        // [BurstCompile]
        // public void OnUpdate(ref SystemState state)
        // {
        //     var config = SystemAPI.GetSingleton<ConfigAuthoring.Config>();
        //
        //     FindBucket(ref state);
        //
        //     MoveToBucket(ref state, ref config);
        //
        //     
        // }

        public void FindBucket(ref SystemState state)
        {
            foreach (var (transform, command, targetBucket, botEntity)
                in SystemAPI.Query<RefRW<WorldTransform>, RefRO<BotCommand>, RefRW<TargetBucket>>()
                .WithEntityAccess())
            {
                if (command.ValueRO.Value != BotAction.GET_BUCKET || targetBucket.ValueRO.value != Entity.Null) continue;

                var minDistance = float.PositiveInfinity;
                var closestBucket = Entity.Null;

                foreach (var (bucket, entity)
                    in SystemAPI.Query<RefRO<WorldTransform>>()
                    .WithAll<Bucket>()
                    .WithNone<BucketActive, BucketFull>()
                    .WithEntityAccess())
                {
                    var distance = math.distancesq(transform.ValueRO.Position, bucket.ValueRO.Position);

                    if (distance < minDistance)
                    {
                        closestBucket = entity;
                        minDistance = distance;
                    }
                }

                if (closestBucket != null)
                {
                    targetBucket.ValueRW.value = closestBucket;
                    Debug.Log($"Closest distance {minDistance}");
                }
            }
        }

        public void MoveToBucket(ref SystemState state, ref ConfigAuthoring.Config config)
        {
            foreach (var (transform, command, targetBucket, botEntity)
                in SystemAPI.Query<RefRW<WorldTransform>, RefRW<BotCommand>, RefRW<TargetBucket>>()
                .WithEntityAccess())
            {
                if (command.ValueRO.Value != BotAction.GET_BUCKET) continue;

                if (state.EntityManager.IsComponentEnabled<BucketActive>(targetBucket.ValueRO.value) == false)
                {
                    var destination = state.EntityManager.GetComponentData<WorldTransform>(targetBucket.ValueRO.value);
                    if (MoveTowards(ref transform.ValueRW.Position, destination.Position, config.botSpeed, .01f))
                    {
                        state.EntityManager.SetComponentEnabled<BucketActive>(targetBucket.ValueRO.value, true);
                        command.ValueRW.Value = BotAction.GOTO_WATER;
                    }
                }
                else
                {
                    targetBucket.ValueRW.value = Entity.Null;
                }
            }
        }

        private bool MoveTowards(ref float3 pos, float3 dest, float speed, float arriveThreshold)
        {
            float3 currPos = pos;
            bool arrivedX = false;
            bool arrivedZ = false;
            float movementSpeed = speed;
           
            // X POSITION
            if (currPos.x < dest.x - arriveThreshold)
            {
                currPos.x += movementSpeed;
            }
            else if (currPos.x > dest.x + arriveThreshold)
            {
                currPos.x -= movementSpeed;
            }
            else
            {
                arrivedX = true;
            }

            // Z POSITION
            if (currPos.z < dest.z - arriveThreshold)
            {
                currPos.z += movementSpeed;
            }
            else if (currPos.z > dest.z + arriveThreshold)
            {
                currPos.z -= movementSpeed;
            }
            else
            {
                arrivedZ = true;
            }

            if (arrivedX && arrivedZ)
            {
                return true;
            }
            else
            {
                pos = dest;
                return false;
            }
        }
    }
}