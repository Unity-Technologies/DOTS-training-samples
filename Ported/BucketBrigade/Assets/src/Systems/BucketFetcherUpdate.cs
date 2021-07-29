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
            RequireSingletonForUpdate<TeamData>();
            m_NotFullBucketsOnFloorQuery = GetEntityQuery(ComponentType.ReadOnly<EcsBucket>(), ComponentType.Exclude<BucketIsHeld>(), ComponentType.Exclude<FillingUpBucketTag>(), ComponentType.Exclude<FullBucketTag>(), ComponentType.ReadOnly<Position>());
            m_EndSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var configValues = GetSingleton<FireSimConfigValues>();
            var timeData = Time;
            
            var ecb = m_EndSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            var concurrentEcb = ecb.AsParallelWriter();
            
            var teamContainer = GetSingletonEntity<TeamData>();
            var teamDatas = EntityManager.GetBuffer<TeamData>(teamContainer);
            
            // Anyone with a BucketFetcherTag who hasn't got a bucket should find a bucket and move towards it.
            if (! m_NotFullBucketsOnFloorQuery.IsEmpty)
            {
                var distanceToPickupBucketSqr = configValues.DistanceToPickupBucket * configValues.DistanceToPickupBucket;
                var bucketEntities = m_NotFullBucketsOnFloorQuery.ToEntityArray(Allocator.TempJob);
                var bucketPositions = m_NotFullBucketsOnFloorQuery.ToComponentDataArray<Position>(Allocator.TempJob);

                Entities.WithBurst()
                    .WithName("MoveTowardsAndPickupBuckets")
                    .WithReadOnly(teamDatas)
                    .WithReadOnly(bucketPositions)
                    .WithReadOnly(bucketEntities)
                    .WithDisposeOnCompletion(bucketPositions)
                    .WithDisposeOnCompletion(bucketEntities)
                    .WithAll<BucketFetcherWorkerTag>()
                    .WithNone<WorkerIsHoldingBucket>()
                    .ForEach((int entityInQueryIndex, Entity workerEntity, ref Position pos, in TeamId teamId) =>
                    {
                        var teamData = teamDatas[teamId.Id];
                        var closestBucketPosition = Utils.GetClosestBucketOutsideTeamWaterSource(teamData.TargetWaterPos, bucketPositions, out _, out var closestBucketEntityIndex, distanceToPickupBucketSqr);
                        
                        if (Utils.MoveToPosition(ref pos, closestBucketPosition.Value, configValues.WorkerSpeed * timeData.DeltaTime))
                        {
                            Utils.PickUpBucket(concurrentEcb, entityInQueryIndex, workerEntity, bucketEntities[closestBucketEntityIndex]);
                        }
                    }).ScheduleParallel();
            }

            // Anyone with a BucketFetcherTag who IS holding a bucket should move towards THEIR OWN TEAM POS, and drop the bucket for that team to use.
            Entities.WithBurst()
                .WithName("DropOffBucketAtTeamPos")
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

                    // If in range, drop it at water location.
                    if (Unity.Burst.CompilerServices.Hint.Likely(distanceToTeam > configValues.DistanceToPickupBucket))
                    {
                        pos.Value += math.normalizesafe(deltaToTeam) * timeData.DeltaTime * configValues.WorkerSpeedWhenHoldingBucket;
                        concurrentEcb.SetComponent(entityInQueryIndex, workerIsHoldingBucket.Bucket, new Position() { Value = pos.Value });
                    }
                    else
                    {
                        Utils.DropBucket(concurrentEcb, entityInQueryIndex, workerEntity, workerIsHoldingBucket.Bucket, teamData.TargetWaterPos);
                    }
                }).ScheduleParallel();

            m_EndSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

    }
}
