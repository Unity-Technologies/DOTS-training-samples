using System;
using src.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace src.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class FullBucketPasserUpdate : BucketWorkerUpdateBase
    {
        protected override QueryBuckets WhichBucketsToQuery { get => QueryBuckets.Full; }
        protected static bool CurveLeft => true;

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<TeamData>();
        }

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
            // Note: Adding 1 for team count, since at the end we'll have a bucket thrower
            var workerCountPerTeam = configValues.WorkerCountPerTeam + 1;

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
                    var firePosition = teamData.TargetFirePos;
                    var targetPosition = GetPositionInTeam(teamData.TargetWaterPos, firePosition, teamPosition.Index, workerCountPerTeam);

                    if (Utils.MoveToPosition(ref pos, targetPosition, timeData.DeltaTime * configValues.WorkerSpeed) && bucketPositions.Length > 0)
                    {
                        Utils.GetClosestBucket(pos.Value, bucketPositions, out var sqrDistanceToBucket, out var closestBucketEntityIndex);
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
                    var firePosition = teamData.TargetFirePos;
                    
                    // Since we're passing a bucket to team mate, specify target position with next index.
                    var targetPosition = GetPositionInTeam(teamData.TargetWaterPos, firePosition, teamPosition.Index + 1, workerCountPerTeam);

                    if (Utils.MoveToPosition(ref pos, targetPosition, timeData.DeltaTime * configValues.WorkerSpeed))
                        Utils.DropBucket(concurrentEcb, entityInQueryIndex, workerEntity, workerIsHoldingBucket.Bucket, targetPosition);
                    else
                        concurrentEcb.SetComponent(entityInQueryIndex, workerIsHoldingBucket.Bucket, new Position { Value = pos.Value });

                }).ScheduleParallel();

            AddECBAsDependency();
        }
    }
}
