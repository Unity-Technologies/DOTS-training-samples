using System;
using src.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace src.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class EmptyBucketPasserUpdate : BucketWorkerUpdateBase
    {
        protected override QueryBuckets WhichBucketsToQuery { get => QueryBuckets.Empty; }

        protected override void OnUpdate()
        {
            if (!TryGetSingletonEntity<TeamData>(out var teamContainerEntity))
                return;

            var teamDatas = EntityManager.GetBuffer<TeamData>(teamContainerEntity);
            var configValues = GetSingleton<FireSimConfigValues>();
            var timeData = Time;
            var bucketPositions = QueryBucketPositions();
            var bucketEntities = QueryBucketEntities();
            var distanceToPickupBucketSqr = configValues.DistanceToPickupBucket * configValues.DistanceToPickupBucket;
            var concurrentEcb = CreateECBParallerWriter();
            // Specifying plus one, so we wouldn't stand on water source 
            var workerCountPerTeam = configValues.WorkerCountPerTeam + 1;

            Entities.WithBurst()
                .WithName("EmptyBucketPassersMoveIntoPosition")
                .WithReadOnly(teamDatas)
                .WithReadOnly(bucketPositions)
                .WithReadOnly(bucketEntities)
                .WithDisposeOnCompletion(bucketPositions)
                .WithDisposeOnCompletion(bucketEntities)
                .WithAll<EmptyBucketPasserTag>()
                .WithNone<WorkerIsHoldingBucket>()
                .ForEach((int entityInQueryIndex, Entity workerEntity, ref Position pos, in TeamId ourTeamId, in TeamPosition teamPosition) =>
                {
                    var teamData = teamDatas[ourTeamId.Id];
                    // TODO: Get Fire position
                    var firePosition = new float2(100, 100);

                    var targetPosition = GetPositionInTeam(firePosition, teamData.TargetWaterPos, teamPosition.Index, workerCountPerTeam);

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
                .WithName("EmptyBucketPassersPassBuckets")
                .WithReadOnly(teamDatas)
                .WithAll<EmptyBucketPasserTag, WorkerIsHoldingBucket>()
                .ForEach((int entityInQueryIndex, Entity workerEntity, ref Position pos, in TeamId ourTeamId, in TeamPosition teamPosition, in WorkerIsHoldingBucket workerIsHoldingBucket) =>
                {
                    var teamData = teamDatas[ourTeamId.Id];

                    // TODO: Get Fire position
                    var firePosition = new float2(100, 100);

                    // Passing bucket to team mater or putting back on the water source ground
                    var targetPosition = GetPositionInTeam(firePosition, teamData.TargetWaterPos, teamPosition.Index + 1, workerCountPerTeam);

                    if (Utils.MoveToPosition(ref pos, targetPosition, timeData.DeltaTime * configValues.WorkerSpeed))
                        Utils.DropBucket(concurrentEcb, entityInQueryIndex, workerEntity, workerIsHoldingBucket);

                    concurrentEcb.SetComponent(entityInQueryIndex, workerIsHoldingBucket.Bucket, new Position() { Value = pos.Value });

                }).ScheduleParallel();
            
            AddECBAsDependency();
        }
    }
}