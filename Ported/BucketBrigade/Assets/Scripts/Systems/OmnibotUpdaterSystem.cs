using System.Collections;
using System.Collections.Generic;
using Authoring;
using Components;
using Enums;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Profiling;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Utilities;

namespace Systems
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    [UpdateAfter(typeof(BucketSpawnerSystem))]
    [UpdateAfter(typeof(OmniBotSpawnerSystem))]
    public partial struct OmnibotUpdaterSystem : ISystem
    {
        static readonly ProfilerMarker getBucketMarker = new ProfilerMarker("Omnibot.GetBucket");
        static readonly ProfilerMarker goToWaterMarker = new ProfilerMarker("Omnibot.GoToWater");
        static readonly ProfilerMarker fillBucketMarker = new ProfilerMarker("Omnibot.FillBucket");
        static readonly ProfilerMarker goToFireMarker = new ProfilerMarker("Omnibot.GoToFire");
        static readonly ProfilerMarker throwBucketMarker = new ProfilerMarker("Omnibot.ThrowBucket");
        
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
            var waterBuffer = SystemAPI.GetSingletonBuffer<ConfigAuthoring.WaterNode>();
            var bucketBuffer = SystemAPI.GetSingletonBuffer<ConfigAuthoring.BucketNode>();

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
                            getBucketMarker.Begin();
                            var foundBucket = Utils.FindBucket(ref state, in botTransform.ValueRO.Position, ref bucketBuffer, false);
                            state.EntityManager.SetComponentData(botEntity, new TargetBucket { Value = foundBucket });
                            
                            if (foundBucket != Entity.Null)
                            {
                                LocalTransform bucketTransform = state.EntityManager.GetComponentData<LocalTransform>(foundBucket);

                                if (Utils.MoveTowards(ref botTransform.ValueRW, bucketTransform.Position, config.botSpeed, config.botArriveThreshold))
                                {
                                    state.EntityManager.SetComponentData(foundBucket, new Bucket { isActive = true, isFull = false });
                                    command.ValueRW.Value = BotAction.GOTO_WATER;
                                }
                            }
                            getBucketMarker.End();
                            break;
                        }
                    case BotAction.GOTO_WATER:
                        {
                            goToWaterMarker.Begin();
                            var foundWater = Utils.FindWater(in botTransform.ValueRO.Position, ref waterBuffer);
                            state.EntityManager.SetComponentData(botEntity, new TargetWater { Value = foundWater });
                            
                            if (foundWater != Entity.Null)
                            {
                                WorldTransform waterTransform = state.EntityManager.GetComponentData<WorldTransform>(foundWater);

                                if (Utils.MoveTowards(ref botTransform.ValueRW, waterTransform.Position, config.botSpeed, config.botArriveThreshold))
                                {
                                    command.ValueRW.Value = BotAction.FILL_BUCKET;
                                }
                            }

                            Utils.UpdateCarriedBucket(ref state, ref targetBucket.Value, ref botTransform.ValueRW);
                            goToWaterMarker.End();
                            break;
                        }
                    case BotAction.FILL_BUCKET:
                        {
                            fillBucketMarker.Begin();
                            var bucketVolume = state.EntityManager.GetComponentData<Volume>(targetBucket.Value);
                            bucketVolume.Value = Mathf.Clamp(bucketVolume.Value + config.bucketFillRate, 0f, config.bucketCapacity);
                            state.EntityManager.SetComponentData(targetBucket.Value, new Volume { Value = bucketVolume.Value });

                            var targetWater = state.EntityManager.GetComponentData<TargetWater>(botEntity);
                            var waterVolume = state.EntityManager.GetComponentData<Volume>(targetWater.Value);

                            waterVolume.Value -= config.bucketFillRate;
                            state.EntityManager.SetComponentData(targetWater.Value, new Volume { Value = waterVolume.Value });
                            
                            if (bucketVolume.Value >= config.bucketCapacity)
                            {
                                state.EntityManager.SetComponentData(targetBucket.Value, new Bucket { isActive = true, isFull = true });
                                state.EntityManager.SetComponentData(botEntity, new TargetWater { Value = Entity.Null });
                                command.ValueRW.Value = BotAction.GOTO_FIRE;
                            }

                            Utils.UpdateFillBucket(ref state, ref targetBucket.Value, ref botTransform.ValueRW, bucketVolume.Value, config.bucketCapacity, config.bucketSizeEmpty, config.bucketSizeFull, config.bucketEmptyColor, config.bucketFullColor);
                            fillBucketMarker.End();
                            break;
                        }
                    case BotAction.GOTO_FIRE:
                        {
                            goToFireMarker.Begin();
                            var foundFire = FindFire(ref state, ref heatMap, in botTransform.ValueRO.Position, config.flashpoint);
                            state.EntityManager.SetComponentData(botEntity, new TargetFlame { Value = foundFire });
                            
                            if (foundFire != Entity.Null)
                            {
                                LocalTransform fireTransform = state.EntityManager.GetComponentData<LocalTransform>(foundFire);
                                if (Utils.MoveTowards(ref botTransform.ValueRW, fireTransform.Position, config.botSpeed, config.botArriveThreshold))
                                {
                                    command.ValueRW.Value = BotAction.THROW_BUCKET;
                                }
                            }
                            Utils.UpdateCarriedBucket(ref state, ref targetBucket.Value, ref botTransform.ValueRW);
                            goToFireMarker.End();
                            break;
                        }
                    case BotAction.THROW_BUCKET:
                        {
                            throwBucketMarker.Begin();
                            var flameCell = state.EntityManager.GetComponentData<FlameCell>(targetFire.Value);

                            Utils.DowseFlameCell(ref heatMap, flameCell.heatMapIndex, config.numRows, config.numColumns, config.coolingStrength, config.coolingStrengthFalloff, config.splashRadius, config.bucketCapacity);
               
                            if (heatMap[flameCell.heatMapIndex].Value < config.flashpoint)
                                state.EntityManager.SetComponentData(botEntity, new TargetFlame { Value = Entity.Null });

                            state.EntityManager.SetComponentData(targetBucket.Value, new Bucket { isActive = true, isFull = false });
                            state.EntityManager.SetComponentData(targetBucket.Value, new Volume { Value = 0 });
                            state.EntityManager.SetComponentData(targetBucket.Value, new URPMaterialPropertyBaseColor() { Value = config.bucketEmptyColor });

                            command.ValueRW.Value = BotAction.GOTO_WATER;
                            Utils.UpdateEmptyBucket(ref state, ref targetBucket.Value, config.bucketSizeEmpty, config.bucketEmptyColor);
                            throwBucketMarker.End();
                            break;
                        }
                }
            }
        }

        public Entity FindFire(ref SystemState state, ref DynamicBuffer<ConfigAuthoring.FlameHeat> heatMap, in float3 botPos, in float flashPoint)
        {
            var minDistance = float.PositiveInfinity;
            var closestFire = Entity.Null;

            int index = 0;
            foreach (var (fireTransform, flameCell, fireEntity)
                     in SystemAPI.Query<RefRO<LocalTransform>, RefRO<FlameCell>>()
                         .WithEntityAccess())
            {
                if (heatMap[index++].Value < flashPoint) continue;

                var distance = math.distancesq(botPos, fireTransform.ValueRO.Position);

                if (distance < minDistance)
                {
                    closestFire = fireEntity;
                    minDistance = distance;
                }
            }

            return closestFire;
        }
    }
}