using System.Collections;
using System.Collections.Generic;
using Authoring;
using Components;
using Enums;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Utilities;
using static Authoring.ConfigAuthoring;
using static UnityEngine.Rendering.DebugUI.Table;

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

            var ecbSysSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var fireECB = ecbSysSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            var waterECB = ecbSysSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var heatNodeArray = CollectionHelper.CreateNativeArray<ConfigAuthoring.FlameHeat>(heatMap.Length, state.WorldUpdateAllocator);
            heatNodeArray.CopyFrom(heatMap.AsNativeArray());

            var waterNodeArray = CollectionHelper.CreateNativeArray<ConfigAuthoring.WaterNode>(waterBuffer.Length, state.WorldUpdateAllocator);
            waterNodeArray.CopyFrom(waterBuffer.AsNativeArray());

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
                            break;
                        }
                    case BotAction.GOTO_WATER:
                        {
                            //var foundWater = Utils.FindWater(in botTransform.ValueRO.Position, ref waterBuffer);
                            //state.EntityManager.SetComponentData(botEntity, new TargetWater { Value = foundWater });

                            //if (foundWater != Entity.Null)
                            //{
                            //    WorldTransform waterTransform = state.EntityManager.GetComponentData<WorldTransform>(foundWater);

                            //    if (Utils.MoveTowards(ref botTransform.ValueRW, waterTransform.Position, config.botSpeed, config.botArriveThreshold))
                            //    {
                            //        command.ValueRW.Value = BotAction.FILL_BUCKET;
                            //    }
                            //}

                            //Utils.UpdateCarriedBucket(ref state, ref targetBucket.Value, ref botTransform.ValueRW);
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
                            //goToFireMarker.Begin();
                            //var foundFire = Utils.FindFireIndex(in botTransform.ValueRO.Position, in heatMap, in flameCells, in config.flashpoint);
                            //state.EntityManager.SetComponentData(botEntity, new TargetFlame { Value = foundFire });
                            
                            //if (foundFire != -1)
                            //{
                            //    LocalTransform fireTransform = state.EntityManager.GetComponentData<LocalTransform>(foundFire);
                            //    if (Utils.MoveTowards(ref botTransform.ValueRW, fireTransform.Position, config.botSpeed, config.botArriveThreshold))
                            //    {
                            //        command.ValueRW.Value = BotAction.THROW_BUCKET;
                            //    }
                            //}
                            //Utils.UpdateCarriedBucket(ref state, ref targetBucket.Value, ref botTransform.ValueRW);
                            //goToFireMarker.End();
                            break;
                        }
                    case BotAction.THROW_BUCKET:
                        {
                            throwBucketMarker.Begin();
                            //var flameCell = state.EntityManager.GetComponentData<FlameCell>(targetFire.Value);

                            Utils.DowseFlameCell(ref heatMap, targetFire.Value, config.numRows, config.numColumns, config.coolingStrength, config.coolingStrengthFalloff, config.splashRadius, config.bucketCapacity);
               
                            if (heatMap[targetFire.Value].Value < config.flashpoint)
                                state.EntityManager.SetComponentData(botEntity, new TargetFlame { Value = -1 });

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

            var gotoWaterJob = new GotoWaterJob
            {
                waterBuffer = waterNodeArray,
                ecb = waterECB,
                botSpeed = config.botSpeed,
                arriveThreshold = config.botArriveThreshold,
                bucketSizeEmpty = config.bucketSizeEmpty
            };

            var gotoFireJob = new GotoFireJob
            {
                fireBuffer = heatNodeArray,
                ecb = fireECB,
                botSpeed = config.botSpeed,
                arriveThreshold = config.botArriveThreshold,
                bucketSizeFull = config.bucketSizeFull,
                flashPoint = config.flashpoint,
                cellSize = config.cellSize,
                numCols = config.numColumns,
                numRows = config.numRows
            };

            state.Dependency = gotoWaterJob.Schedule(state.Dependency);
            state.Dependency = gotoFireJob.ScheduleParallel(state.Dependency);
        }
    }

    [BurstCompile]
    [WithAll(typeof(BotOmni))]
    public partial struct GotoWaterJob : IJobEntity
    {
        [ReadOnly] public NativeArray<ConfigAuthoring.WaterNode> waterBuffer;
        public EntityCommandBuffer ecb;
        public float botSpeed;
        public float arriveThreshold;
        public float bucketSizeEmpty;

        public void Execute(ref LocalTransform transform, ref BotCommand botCommand, ref TargetBucket targetBucket, Entity entity)
        {
            if (botCommand.Value != BotAction.GOTO_WATER) return;

            var foundIndex = Utils.FindWaterIndex(in transform.Position, ref waterBuffer);
            if (foundIndex != -1)
            {
                var foundWater = waterBuffer[foundIndex];
                ecb.SetComponent(entity, new TargetWater { Value = foundWater.Node });

                var newPosition = Utils.MoveTowards2(in transform.Position, in foundWater.Position, botSpeed, arriveThreshold);
                ecb.SetComponent(entity, LocalTransform.FromPosition(newPosition));
                ecb.SetComponent(targetBucket.Value, LocalTransform.FromPositionRotationScale(newPosition + new float3(0, .5f, 0), Quaternion.identity, bucketSizeEmpty));

                if (Utils.IsClose(newPosition, foundWater.Position, arriveThreshold))
                {
                    ecb.SetComponent(entity, new BotCommand { Value = BotAction.FILL_BUCKET });
                }
            }
        }
    }

    [BurstCompile]
    [WithAll(typeof(BotOmni))]
    public partial struct GotoFireJob : IJobEntity
    {
        [ReadOnly] public NativeArray<ConfigAuthoring.FlameHeat> fireBuffer;
        public EntityCommandBuffer.ParallelWriter ecb;
        public float botSpeed;
        public float arriveThreshold;
        public float bucketSizeFull;
        public float flashPoint;
        public float cellSize;
        public int numCols;
        public int numRows;

        public void Execute(ref LocalTransform transform, ref BotCommand botCommand, ref TargetBucket targetBucket, Entity entity, [ChunkIndexInQuery] int chunkIndex)
        {
            if (botCommand.Value != BotAction.GOTO_FIRE) return;

            var (foundIndex, foundPos) = Utils.FindFireIndex(in transform.Position, in fireBuffer, flashPoint, cellSize, numCols, numRows);
            if (foundIndex != -1)
            {
                ecb.SetComponent(chunkIndex, entity, new TargetFlame { Value = foundIndex });

                var newPosition = Utils.MoveTowards2(in transform.Position, in foundPos, botSpeed, arriveThreshold);
                ecb.SetComponent(chunkIndex, entity, LocalTransform.FromPosition(newPosition));
                ecb.SetComponent(chunkIndex, targetBucket.Value, LocalTransform.FromPositionRotationScale(newPosition + new float3(0, .5f, 0), Quaternion.identity, bucketSizeFull));

                if (Utils.IsClose(newPosition, foundPos, arriveThreshold))
                {
                    ecb.SetComponent(chunkIndex, entity, new BotCommand { Value = BotAction.THROW_BUCKET });
                }
            }
        }
    }
}