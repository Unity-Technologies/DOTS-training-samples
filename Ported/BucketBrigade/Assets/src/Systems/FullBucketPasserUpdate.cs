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
                    if (!teamData.IsValid) return;
                    
                    var firePosition = teamData.TargetFirePos;
                    var targetPosition = GetPositionInTeam(teamData.TargetWaterPos, firePosition, teamPosition.Index, workerCountPerTeam);
                    var targetBucketPosition = GetPositionInTeam(teamData.TargetWaterPos, firePosition, teamPosition.Index + 1, workerCountPerTeam);

                    // The first worker should be able to grab more full buckets from around.
                    var isFirstWorker = teamPosition.Index <= 0;
                    var reachSqrd = distanceToPickupBucketSqr;
                    if (isFirstWorker) reachSqrd *= 2.5f;
                    
                    var bucketIndex = MoveToPositionAndPickupBucket(ref pos,
                        teamData.TargetWaterPos,
                        targetPosition,
                        targetBucketPosition,
                        timeData.DeltaTime * configValues.WorkerSpeedWhenHoldingBucket,
                        bucketPositions,
                        reachSqrd, 
                        false);

                    if (bucketIndex >= 0)
                    {
                        // NW: Knowing that ConcurrentECB's require a sort order, we can pass in our team position to make sure our teammates DOWN THE LINE have a higher priority of picking up the bucket. 
                        var hackedSortPosition = teamPosition.Index;
                        Utils.AddPickUpBucketRequest(concurrentEcb, hackedSortPosition, workerEntity, bucketEntities[bucketIndex], Utils.PickupRequestType.Carry);
                    }
                }).ScheduleParallel();


            Entities.WithBurst()
                .WithName("FullBucketPassersPassBuckets")
                .WithReadOnly(teamDatas)
                .WithAll<FullBucketPasserTag, WorkerIsHoldingBucket>()
                .ForEach((int entityInQueryIndex, Entity workerEntity, ref Position pos, in TeamId ourTeamId, in TeamPosition teamPosition, in WorkerIsHoldingBucket workerIsHoldingBucket) =>
                {
                    var teamData = teamDatas[ourTeamId.Id];
                    if (!teamData.IsValid) return;
                    
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
