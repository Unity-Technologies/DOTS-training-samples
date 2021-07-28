using System;
using src.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace src.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class BucketFetcherUpdate : SystemBase
    {
        EntityQuery m_NotFullBucketsOnFloorQuery;
        EndSimulationEntityCommandBufferSystem m_EndSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<FireSimConfigValues>();
            m_NotFullBucketsOnFloorQuery = GetEntityQuery(ComponentType.ReadOnly<EcsBucket>(), ComponentType.Exclude<BucketIsHeld>(), ComponentType.Exclude<FillUpBucketTag>(), ComponentType.Exclude<FullBucketTag>(), ComponentType.ReadOnly<Position>());
            m_EndSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var configValues = GetSingleton<FireSimConfigValues>();
            var timeData = Time;
            
            var ecb = m_EndSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            var concurrentEcb = ecb.AsParallelWriter();
            
            // Anyone with a BucketFetcherTag who hasn't got a bucket should find a bucket and move towards it.
            if (! m_NotFullBucketsOnFloorQuery.IsEmpty)
            {
                var distanceToPickupBucketSqr = configValues.DistanceToPickupBucket * configValues.DistanceToPickupBucket;
                var bucketEntities = m_NotFullBucketsOnFloorQuery.ToEntityArray(Allocator.TempJob);
                var bucketPositions = m_NotFullBucketsOnFloorQuery.ToComponentDataArray<Position>(Allocator.TempJob);

                
                Entities.WithBurst()
                    .WithName("MoveTowardsAndPickupBuckets")
                    .WithReadOnly(bucketPositions)
                    .WithReadOnly(bucketEntities)
                    .WithDisposeOnCompletion(bucketPositions)
                    .WithDisposeOnCompletion(bucketEntities)
                    .WithAll<BucketFetcherWorkerTag>()
                    .WithNone<WorkerIsHoldingBucket>()
                    .ForEach((int entityInQueryIndex, Entity workerEntity, ref Position pos) =>
                    {
                        var closestBucketPosition = Utils.GetClosestBucket(pos, bucketPositions, out var sqrDistanceToBucket, out var closestBucketEntityIndex);
                        var deltaToBucket = closestBucketPosition.Value - pos.Value;
                        if (sqrDistanceToBucket < distanceToPickupBucketSqr)
                        {
                            Utils.PickUpBucket(concurrentEcb, entityInQueryIndex, workerEntity, bucketEntities[closestBucketEntityIndex]);
                        }
                        else
                        {
                            pos.Value += math.normalizesafe(deltaToBucket) * timeData.DeltaTime * configValues.WorkerSpeed;
                        }
                    }).ScheduleParallel();
            }

            // Anyone with a BucketFetcherTag who IS holding a bucket should move towards THEIR OWN TEAM POS, and drop the bucket for that team to use.
            if (TryGetSingletonEntity<TeamData>(out var teamContainerEntity))
            {
                var teamDatas = EntityManager.GetBuffer<TeamData>(teamContainerEntity);
                
                Entities.WithBurst()
                    .WithName("DropOffFullBucketAtTeamPos")
                    .WithReadOnly(teamDatas)
                    .WithAll<BucketFetcherWorkerTag, WorkerIsHoldingBucket>()
                    .WithNone<OmniWorkerTag>()
                    .ForEach((int entityInQueryIndex, Entity workerEntity, ref Position pos, in TeamId ourTeamId, in WorkerIsHoldingBucket workerIsHoldingBucket) =>
                    {
                        // First we find our teams water pos.
                        var teamData = teamDatas[ourTeamId.Id];

                        // Then we move towards it.
                        var deltaToTeam = teamData.TargetWaterPos - pos.Value;
                        var distanceToTeam = math.length(deltaToTeam);
                        
                        if (Unity.Burst.CompilerServices.Hint.Likely(distanceToTeam > configValues.DistanceToPickupBucket))
                        {
                            // If in range, drop it at water location.
                            pos.Value += math.normalizesafe(deltaToTeam) * timeData.DeltaTime * configValues.WorkerSpeedWhenHoldingBucket;
                            // Update bucket position
                            concurrentEcb.SetComponent(entityInQueryIndex, workerIsHoldingBucket.Bucket, new Position() { Value = pos.Value});
                        }
                        else
                        {
                            concurrentEcb.SetComponent(entityInQueryIndex, workerIsHoldingBucket.Bucket, new Position() { Value = teamData.TargetWaterPos });
                            concurrentEcb.AddComponent<FillUpBucketTag>(entityInQueryIndex, workerIsHoldingBucket.Bucket);
                            // // DO SOME ECB STUFF.
                           
                        }
                    }).ScheduleParallel();
            }

            Entities
                .WithName("FillUpAndDropBuckets")
                .WithBurst()
                .WithAll<FillUpBucketTag>()
                .ForEach((Entity bucketEntity, int entityInQueryIndex, ref EcsBucket bucket, in BucketIsHeld bucketIsHeld) =>
            {
                bucket.WaterLevel += timeData.DeltaTime / configValues.WaterFillUpDuration;
                if (Unity.Burst.CompilerServices.Hint.Unlikely(bucket.WaterLevel >= 1))
                {
                    bucket.WaterLevel = 1f;
                    
                    // Set bucket tags:
                    concurrentEcb.AddComponent<FullBucketTag>(entityInQueryIndex, bucketEntity);
                    concurrentEcb.RemoveComponent<FillUpBucketTag>(entityInQueryIndex, bucketEntity);
                    
                    // Drop the bucket:
                    concurrentEcb.RemoveComponent<BucketIsHeld>(entityInQueryIndex, bucketEntity);
                    concurrentEcb.RemoveComponent<WorkerIsHoldingBucket>(entityInQueryIndex, bucketIsHeld.WorkerHoldingThis);
                }
            }).ScheduleParallel();
            
            m_EndSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

    }
}
