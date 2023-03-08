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
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ConfigAuthoring.Config>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<ConfigAuthoring.Config>();

            FindBucket(ref state);

            MoveToBucket(ref state);
        }

        public void FindBucket(ref SystemState state)
        {
            foreach (var (transform, command, botEntity)
                in SystemAPI.Query<RefRW<WorldTransform>, RefRO<BotCommand>>()
                .WithEntityAccess())
            {
                if (command.ValueRO.Value != BotAction.GET_BUCKET) continue;

                var minDistance = float.PositiveInfinity;
                var minPosition = float3.zero;
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
                        minPosition = bucket.ValueRO.Position;
                    }
                }

                if (closestBucket != null)
                {
                    state.EntityManager.SetComponentData(botEntity, new TargetBucket { value = closestBucket });
                    Debug.Log($"Closest distance {minDistance}");
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
    }
}