using System;
using src.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace src.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class FullBucketPasserUpdate : SystemBase
    {
        EntityQuery m_FullBucketsOnFloorQuery;
        EndSimulationEntityCommandBufferSystem m_EndSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<FireSimConfigValues>();

            m_FullBucketsOnFloorQuery = GetEntityQuery(
                ComponentType.ReadOnly<EcsBucket>(),
                ComponentType.ReadOnly<FullBucketTag>(),
                ComponentType.Exclude<BucketIsHeld>(), 
                ComponentType.Exclude<FillUpBucketTag>(),
                ComponentType.ReadOnly<Position>());

            m_EndSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            if (!TryGetSingletonEntity<TeamData>(out var teamContainerEntity))
                return;
            
            var teamDatas = EntityManager.GetBuffer<TeamData>(teamContainerEntity);
            var configValues = GetSingleton<FireSimConfigValues>();
            var timeData = Time;
            var bucketPositions = m_FullBucketsOnFloorQuery.ToComponentDataArray<Position>(Allocator.TempJob);
            var bucketEntities = m_FullBucketsOnFloorQuery.ToEntityArray(Allocator.TempJob);
            var distanceToPickupBucketSqr = configValues.DistanceToPickupBucket * configValues.DistanceToPickupBucket;
            var ecb = m_EndSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            var concurrentEcb = ecb.AsParallelWriter();

            Entities.WithBurst()
                .WithName("FullBucketPassersMoveIntoPosition")
                .WithReadOnly(teamDatas)
                .WithReadOnly(bucketPositions)
                .WithReadOnly(bucketEntities)
                .WithDisposeOnCompletion(bucketPositions)
                .WithDisposeOnCompletion(bucketEntities)
                .WithAll<FullBucketPasserTag>()
                .WithNone<WorkerIsHoldingBucket>()
                .ForEach((int entityInQueryIndex, Entity workerEntity, ref Position pos, in TeamId ourTeamId, in TeamPosition teamPosition) =>
                {
                    var teamData = teamDatas[ourTeamId.Id];
                    // TODO: Get Fire position
                    var firePosition = new float2(100, 100);
                    var targetPosition = GetPositionInTeam(teamData.TargetWaterPos, firePosition, teamPosition.Index, configValues.WorkerCountPerTeam);

                    if (Utils.MoveToPosition(ref pos, targetPosition, timeData.DeltaTime * configValues.WorkerSpeed) && bucketPositions.Length > 0)
                    {
                        Utils.GetClosestBucket(pos, bucketPositions, out var sqrDistanceToBucket, out var closestBucketEntityIndex);
                        // Found a bucket, start carrying to team mate
                        if (sqrDistanceToBucket < distanceToPickupBucketSqr)
                        {
                            Utils.PickUpBucket(concurrentEcb, entityInQueryIndex, workerEntity, bucketEntities[closestBucketEntityIndex]);
                        }

                    }

                }).ScheduleParallel();


            Entities.WithBurst()
                .WithName("FullBucketPassersPassBuckets")
                .WithReadOnly(teamDatas)
                .WithAll<FullBucketPasserTag, WorkerIsHoldingBucket>()
                .ForEach((int entityInQueryIndex, Entity workerEntity, ref Position pos, in TeamId ourTeamId, in TeamPosition teamPosition, in WorkerIsHoldingBucket workerIsHoldingBucket) =>
                {
                    var teamData = teamDatas[ourTeamId.Id];

                    // TODO: Get Fire position
                    var firePosition = new float2(100, 100);

                    // Since we're passing a bucket to team mate, specify target position with next index.
                    var targetPosition = GetPositionInTeam(teamData.TargetWaterPos, firePosition, teamPosition.Index + 1, configValues.WorkerCountPerTeam);

                    if (Utils.MoveToPosition(ref pos, targetPosition, timeData.DeltaTime * configValues.WorkerSpeed))
                        Utils.DropBucket(concurrentEcb, entityInQueryIndex, workerEntity, workerIsHoldingBucket);

                    concurrentEcb.SetComponent(entityInQueryIndex, workerIsHoldingBucket.Bucket, new Position() { Value = pos.Value });

                }).ScheduleParallel();

            m_EndSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
        
        static float2 GetPositionInTeam(float2 startPosition, float2 endPosition, int id, int teamCount)
        {
            float t = (float) id / (float) (teamCount - 1);

            var position =  math.lerp(startPosition, endPosition, t);
            // Give it some curve
            var curveOffset = math.normalizesafe(endPosition - startPosition) * 10.0f;
            var tmp = curveOffset.x;
            curveOffset.x = -curveOffset.y;
            curveOffset.y = tmp;
            position += math.sin(t * math.PI) * curveOffset;

            return position;
        }
    }
}
