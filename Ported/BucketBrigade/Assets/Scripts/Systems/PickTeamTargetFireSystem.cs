using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class PickTeamTargetFireSystem : SystemBase
{
    private const float kWaterFireLineWidth = 1.5f;

    protected override void OnCreate()
    {
        // Wait for the specified instanciations
        RequireSingletonForUpdate<Spawner>();
        RequireSingletonForUpdate<Heat>();
    }

    protected override void OnUpdate()
    {
        var spawner = GetSingleton<Spawner>();
        var heatSingleton = EntityManager.GetBuffer<Heat>(GetSingletonEntity<Heat>());
        var gridSize = GetComponent<Spawner>(GetSingletonEntity<Spawner>()).GridSize;

        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var parallelEcb = ecb.AsParallelWriter();

        Entities
            .WithNativeDisableContainerSafetyRestriction(heatSingleton)
            .ForEach(
                (int entityInQueryIndex, ref Team team) =>
                {
                    // Only change if current is no longer on fire (or uninitialized)
                    int2 currentIndex = team.targetFirePosition;
                    if (currentIndex.x < 0 || currentIndex.y < 0 || heatSingleton[currentIndex.x + currentIndex.y * gridSize].Value < FirePropagationSystem.HeatBurningLevel)
                    {
                        // Initialize closest to current fire distance
                        float closestFireDistanceSq = float.MaxValue;

                        //TODO: find closest fire cell, then hottest fire cell (Mike)
                        //TODO : only go through the cell that are on fire (Optim)

                        int2 closestFirePosition = new int2(-1, -1);

                        // look at every cell, and if one of them is on fire, look at the distance
                        for (int i = 0; i < heatSingleton.Length; i++)
                        {
                            // TODO : add bool for heat if on fire
                            if (heatSingleton[i].Value > FirePropagationSystem.HeatBurningLevel)
                            {
                                int2 firePosition = new int2(
                                    i % gridSize,
                                    i / gridSize);

                                var currentDistanceSq =
                                    math.pow(firePosition.y - team.targetWaterPosition.z, 2) +
                                    math.pow(firePosition.x - team.targetWaterPosition.x, 2);

                                if (currentDistanceSq < closestFireDistanceSq)
                                {
                                    closestFireDistanceSq = currentDistanceSq;
                                    closestFirePosition = firePosition;
                                }
                            }
                        }

                        bool targetFireChanged = closestFireDistanceSq != float.MaxValue;
                        if (targetFireChanged)
                        {
                            // Updates Fire position
                            team.targetFirePosition = closestFirePosition;

                            // Set Bucket fetcher target position as water position - this can happen in the spawner (moved)
                            // parallelEcb.SetComponent<TargetPosition>(entityInQueryIndex, team.bucketFetcherWorker, new TargetPosition { Value = team.targetWaterPosition }) ;
                            
                            var firePosition = new float3(team.targetFirePosition.x, 1, team.targetFirePosition.y);
                            var waterFireLine = firePosition - team.targetWaterPosition;
                            var waterFireLineLength = math.length(waterFireLine);
                            var waterFireLineLeft = math.normalize(new float3(-waterFireLine.z, 0, waterFireLine.x)) * kWaterFireLineWidth;
                            var space =  1.0f / (float)(spawner.FullBucketWorkerPerTeamCount);
                            int index = 0; // 0 also for bucketfetcher, occupy same position

                            for (var indexedBucketWorker = team.fullBucketWorker; indexedBucketWorker != Entity.Null; index++)
                            {
                                // Spread workers along fire-water line
                                var targetPosition = GetComponent<TargetPosition>(indexedBucketWorker);
                                var segmentRatio = space * index;
                                var targetPositionValue = team.targetWaterPosition + waterFireLine * segmentRatio + math.sin(segmentRatio * math.PI)* waterFireLineLeft;

                                parallelEcb.SetComponent<TargetOriginalPosition>(entityInQueryIndex, indexedBucketWorker, new TargetOriginalPosition { Value = targetPositionValue });
                                parallelEcb.SetComponent<TargetPosition>(entityInQueryIndex, indexedBucketWorker, new TargetPosition { Value = targetPositionValue });

                                // Switch to next worker
                                var nextWorker = GetComponent<FullBucketWorker>(indexedBucketWorker).nextWorker;
                                if (nextWorker == Entity.Null)
                                    parallelEcb.SetComponent<LastWorker>(entityInQueryIndex, indexedBucketWorker, new LastWorker { targetPosition = firePosition });
                                indexedBucketWorker = nextWorker;
                            }

                            waterFireLine = team.targetWaterPosition - firePosition;
                            waterFireLineLeft = math.normalize(new float3(-waterFireLine.z, 0, waterFireLine.x)) * kWaterFireLineWidth;
                            space = 1.0f / (float)(spawner.EmptyBucketWorkerPerTeamCount);
                            index = 0; // 0 also for bucketfetcher, occupy same position

                            for (var indexedBucketWorker = team.emptyBucketWorker; indexedBucketWorker != Entity.Null; index++)
                            {
                                // Spread workers along fire-water line
                                var targetPosition = GetComponent<TargetPosition>(indexedBucketWorker);
                                var segmentRatio = space * index;
                                var targetPositionValue = firePosition + waterFireLine * segmentRatio + math.sin(segmentRatio * math.PI ) * waterFireLineLeft;

                                parallelEcb.SetComponent<TargetOriginalPosition>(entityInQueryIndex, indexedBucketWorker, new TargetOriginalPosition { Value = targetPositionValue });
                                parallelEcb.SetComponent<TargetPosition>(entityInQueryIndex, indexedBucketWorker, new TargetPosition { Value = targetPositionValue });

                                // Switch to next worker
                                indexedBucketWorker = GetComponent<EmptyBucketWorker>(indexedBucketWorker).nextWorker;
                            }
                        }
                    }
                }).ScheduleParallel();

        this.Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}