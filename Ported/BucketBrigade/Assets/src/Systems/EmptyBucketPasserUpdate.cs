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
                    if (!teamData.IsValid) return;
                    
                    var firePosition = teamData.TargetFirePos;

                    var targetPosition = GetPositionInTeam(firePosition, teamData.TargetWaterPos, teamPosition.Index, workerCountPerTeam);
                    var targetBucketPosition = GetPositionInTeam(firePosition, teamData.TargetWaterPos, teamPosition.Index + 1, workerCountPerTeam);
                    var isLastWorker = teamPosition.Index >= workerCountPerTeam - 1;
                    var bucketIndex = MoveToPositionAndPickupBucket(ref pos,
                        teamData.TargetWaterPos,
                        targetPosition,
                        targetBucketPosition,
                        timeData.DeltaTime * configValues.WorkerSpeed,
                        bucketPositions,
                        distanceToPickupBucketSqr, 
                        true);

                    if (bucketIndex >= 0)
                    {
                        // NW: Knowing that ConcurrentECB's require a sort order, we can pass in our team position to make sure our teammates DOWN THE LINE have a higher priority of picking up the bucket. 
                        var hackedSortPosition = teamPosition.Index;
                        Utils.AddPickUpBucketRequest(concurrentEcb, hackedSortPosition, workerEntity, bucketEntities[bucketIndex], Utils.PickupRequestType.Carry);
                    }


                }).ScheduleParallel();

            
            Entities.WithBurst()
                .WithName("EmptyBucketPassersPassBuckets")
                .WithReadOnly(teamDatas)
                .WithAll<EmptyBucketPasserTag, WorkerIsHoldingBucket>()
                .ForEach((int entityInQueryIndex, Entity workerEntity, ref Position pos, in TeamId ourTeamId, in TeamPosition teamPosition, in WorkerIsHoldingBucket workerIsHoldingBucket) =>
                {
                    var teamData = teamDatas[ourTeamId.Id];
                    if (!teamData.IsValid) return;
                    
                    var firePosition = teamData.TargetFirePos;

                    // Passing bucket to team mater or putting back on the water source ground
                    var targetPosition = GetPositionInTeam(firePosition, teamData.TargetWaterPos, teamPosition.Index + 1, workerCountPerTeam);

                    if (Utils.MoveToPosition(ref pos, targetPosition, timeData.DeltaTime * configValues.WorkerSpeed))
                        Utils.DropBucket(concurrentEcb, entityInQueryIndex, workerEntity, workerIsHoldingBucket.Bucket, targetPosition);
                    else
                        concurrentEcb.SetComponent(entityInQueryIndex, workerIsHoldingBucket.Bucket, new Position() { Value = pos.Value });

                }).ScheduleParallel();
            
            AddECBAsDependency();
        }
    }
}