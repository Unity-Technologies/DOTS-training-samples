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
            // TODO: Use the Worker base class as query should be the same as EmptyBucket query.
            m_NotFullBucketsOnFloorQuery = GetEntityQuery(ComponentType.ReadOnly<EcsBucket>(), ComponentType.Exclude<BucketIsHeld>(), ComponentType.Exclude<PickUpBucketRequest>(), ComponentType.Exclude<FillingUpBucketTag>(), ComponentType.ReadOnly<Position>());
            m_EndSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var configValues = GetSingleton<FireSimConfigValues>();
            var distanceToPickupBucketSqr = configValues.DistanceToPickupBucket * configValues.DistanceToPickupBucket;
            var timeData = Time;
            
            var ecb = m_EndSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            var concurrentEcb = ecb.AsParallelWriter();
            
            var teamContainer = GetSingletonEntity<TeamData>();
            var teamDatas = EntityManager.GetBuffer<TeamData>(teamContainer);
            
            // Anyone with a BucketFetcherTag who hasn't got a bucket should find a bucket and move towards it.
            if (! m_NotFullBucketsOnFloorQuery.IsEmpty)
            {
             
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
                        var closestBucketPosition = Utils.GetClosestBucketOutsideTeamWaterSource(pos.Value, teamData.TargetWaterPos, bucketPositions, out _, out var closestBucketEntityIndex, distanceToPickupBucketSqr);

                        if (closestBucketEntityIndex >= 0)
                        {
                            Debug.DrawLine(Utils.To3D(pos.Value), Utils.To3D(closestBucketPosition.Value), Color.blue);
                            
                            if (teamData.IsValid && Utils.MoveToPosition(ref pos, closestBucketPosition.Value, configValues.WorkerSpeed * timeData.DeltaTime))
                            {
                                Utils.AddPickUpBucketRequest(concurrentEcb, entityInQueryIndex, workerEntity, bucketEntities[closestBucketEntityIndex], Utils.PickupRequestType.Carry);
                            }
                        }
                        // Else idle as no buckets left.
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
                    if (!teamData.IsValid) return;
                    
                    // Then we move towards it.
                    var deltaToTeam = teamData.TargetWaterPos - pos.Value;
                    var distanceToTeamSqr = math.lengthsq(deltaToTeam);

                    // If in range, drop it at water location.
                    if (Unity.Burst.CompilerServices.Hint.Likely(distanceToTeamSqr > distanceToPickupBucketSqr))
                    {
                        pos.Value += math.normalizesafe(deltaToTeam) * timeData.DeltaTime * configValues.WorkerSpeedWhenHoldingBucket;
                        concurrentEcb.SetComponent(entityInQueryIndex, workerIsHoldingBucket.Bucket, new Position { Value = pos.Value });
                    }
                    else
                    {
                        // Drop it NEAR the middle of the water source, but not exactly AT it. Allows us to see hundreds of stacked buckets.
                        var dropOffset = (teamData.TargetWaterPos - pos.Value) * 0.5f;
                        Utils.DropBucket(concurrentEcb, entityInQueryIndex, workerEntity, workerIsHoldingBucket.Bucket, pos.Value + dropOffset);
                    }
                }).ScheduleParallel();

            m_EndSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

    }
}
