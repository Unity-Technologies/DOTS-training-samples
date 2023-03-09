using System.Collections;
using System.Collections.Generic;
using Components;
using Enums;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
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
            var heatMap = SystemAPI.GetSingletonBuffer<ConfigAuthoring.FlameHeat>();

            foreach (var (botTransform, command, botEntity) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRW<BotCommand>>()
                     .WithAll<BotOmni>()
                     .WithEntityAccess())
            {
                var targetBucket = state.EntityManager.GetComponentData<TargetBucket>(botEntity);
                var targetFire = state.EntityManager.GetComponentData<TargetFlame>(botEntity);

                switch (command.ValueRW.Value)
                {
                    case BotAction.GET_BUCKET:
                        {
                            var foundBucket = FindBucket(ref state, in botTransform.ValueRO.Position);
                            state.EntityManager.SetComponentData(botEntity, new TargetBucket { value = foundBucket });
                            
                            if (foundBucket != Entity.Null)
                            {
                                LocalTransform bucketTransform = state.EntityManager.GetComponentData<LocalTransform>(foundBucket);

                                if (Utils.MoveTowards(ref botTransform.ValueRW, bucketTransform.Position, config.botSpeed, config.botArriveThreshold))
                                {
                                    state.EntityManager.SetComponentData(foundBucket, new Bucket { isActive = true, isFull = false });
                                    command.ValueRW.Value = BotAction.GOTO_WATER;
                                }
                            }
                            break;
                        }
                    case BotAction.GOTO_WATER:
                        {
                            var foundWater = FindWater(ref state, in botTransform.ValueRO.Position);
                            state.EntityManager.SetComponentData(botEntity, new TargetWater { value = foundWater });
                            
                            if (foundWater != Entity.Null)
                            {
                                WorldTransform waterTransform = state.EntityManager.GetComponentData<WorldTransform>(foundWater);

                                if (Utils.MoveTowards(ref botTransform.ValueRW, waterTransform.Position, config.botSpeed, config.botArriveThreshold))
                                {
                                    command.ValueRW.Value = BotAction.FILL_BUCKET;
                                }
                            }

                            Utils.UpdateCarriedBucket(ref state, ref targetBucket.value, ref botTransform.ValueRW);
                            break;
                        }
                    case BotAction.FILL_BUCKET:
                        {
                            var bucketVolume = state.EntityManager.GetComponentData<Volume>(targetBucket.value);
                            bucketVolume.value = Mathf.Clamp(bucketVolume.value + config.bucketFillRate, 0f, config.bucketCapacity);
                            state.EntityManager.SetComponentData(targetBucket.value, new Volume { value = bucketVolume.value });

                            var targetWater = state.EntityManager.GetComponentData<TargetWater>(botEntity);
                            var waterVolume = state.EntityManager.GetComponentData<Volume>(targetWater.value);

                            waterVolume.value -= config.bucketFillRate;
                            state.EntityManager.SetComponentData(targetWater.value, new Volume { value = waterVolume.value });
                            
                            if (bucketVolume.value >= config.bucketCapacity)
                            {
                                state.EntityManager.SetComponentData(targetBucket.value, new Bucket { isActive = true, isFull = true });
                                state.EntityManager.SetComponentData(botEntity, new TargetWater { value = Entity.Null });
                                command.ValueRW.Value = BotAction.GOTO_FIRE;
                            }

                            Utils.UpdateFillBucket(ref state, ref targetBucket.value, ref botTransform.ValueRW, bucketVolume.value, config.bucketCapacity, config.bucketSizeEmpty, config.bucketSizeFull, config.bucketEmptyColor, config.bucketFullColor);
                            
                            break;
                        }
                    case BotAction.GOTO_FIRE:
                        {
                            var foundFire = FindFire(ref state, in botTransform.ValueRO.Position);
                            state.EntityManager.SetComponentData(botEntity, new TargetFlame { value = foundFire });
                            
                            if (foundFire != Entity.Null)
                            {
                                LocalTransform fireTransform = state.EntityManager.GetComponentData<LocalTransform>(foundFire);
                                if (Utils.MoveTowards(ref botTransform.ValueRW, fireTransform.Position, config.botSpeed, config.botArriveThreshold))
                                {
                                    command.ValueRW.Value = BotAction.THROW_BUCKET;
                                }
                            }
                            Utils.UpdateCarriedBucket(ref state, ref targetBucket.value, ref botTransform.ValueRW);

                            break;
                        }
                    case BotAction.THROW_BUCKET:
                        {
                            var flameCell = state.EntityManager.GetComponentData<FlameCell>(targetFire.value);

                            Utils.DowseFlameCell(ref heatMap, flameCell.heatMapIndex, config.numRows, config.numColumns, config.coolingStrength, config.coolingStrengthFalloff, config.splashRadius, config.bucketCapacity);
               
                            if (heatMap[flameCell.heatMapIndex].Value < config.flashpoint)
                                state.EntityManager.SetComponentData(botEntity, new TargetFlame { value = Entity.Null });

                            state.EntityManager.SetComponentData(targetBucket.value, new Bucket { isActive = true, isFull = false });
                            state.EntityManager.SetComponentData(targetBucket.value, new Volume { value = 0 });
                            state.EntityManager.SetComponentData(targetBucket.value, new URPMaterialPropertyBaseColor() { Value = config.bucketEmptyColor });

                            command.ValueRW.Value = BotAction.GOTO_WATER;
                            Utils.UpdateEmptyBucket(ref state, ref targetBucket.value, config.bucketSizeEmpty, config.bucketEmptyColor);

                            break;
                        }
                }
            }
        }


        public Entity FindBucket(ref SystemState state, in float3 botPos, bool wantsFull = false)
        {
            var minDistance = float.PositiveInfinity;
            var closestBucket = Entity.Null;

            foreach (var (bucketTransform, bucket, bucketEntity)
                     in SystemAPI.Query<RefRO<LocalTransform>, RefRO<Bucket>>()
                         .WithEntityAccess())
            {
                if (bucket.ValueRO.isActive == true) continue;

                var distance = math.distancesq(botPos, bucketTransform.ValueRO.Position);

                if (distance < minDistance)
                {
                    closestBucket = bucketEntity;
                    minDistance = distance;
                }
            }

            return closestBucket;
        }

        public Entity FindWater(ref SystemState state, in float3 botPos)
        {
            var minDistance = float.PositiveInfinity;
            var closestWater = Entity.Null;

            foreach (var (waterTransform, waterEntity)
                     in SystemAPI.Query<RefRO<WorldTransform>>()
                         .WithAll<WaterAuthoring.Water>()
                         .WithEntityAccess())
            {
                var distance = math.distancesq(botPos, waterTransform.ValueRO.Position);

                if (distance < minDistance)
                {
                    closestWater = waterEntity;
                    minDistance = distance;
                }
            }

            return closestWater;
        }

        public Entity FindFire(ref SystemState state, in float3 botPos)
        {
            var minDistance = float.PositiveInfinity;
            var closestFire = Entity.Null;

            foreach (var (fireTransform, flameCell, fireEntity)
                     in SystemAPI.Query<RefRO<LocalTransform>, RefRO<FlameCell>>()
                         .WithEntityAccess())
            {
                if (flameCell.ValueRO.isOnFire == false) continue;

                var distance = math.distancesq(botPos, fireTransform.ValueRO.Position);

                if (distance < minDistance)
                {
                    closestFire = fireEntity;
                    minDistance = distance;
                }
            }

            if (closestFire != null) Debug.Log("Fire Found");

            return closestFire;
        }
    }
}