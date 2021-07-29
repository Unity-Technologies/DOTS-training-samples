using System;
using src.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace src.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class BucketFillerUpdate : BucketWorkerUpdateBase
    {
        protected override QueryBuckets WhichBucketsToQuery => QueryBuckets.Empty;
        
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
            var concurrentEcb = CreateECBParallerWriter();
            
            Entities
                .WithName("FillUpAndDropBucket")
                .WithBurst()
                .WithAll<FillingUpBucketTag>()
                .ForEach((Entity bucketEntity, int entityInQueryIndex, ref EcsBucket bucket, in BucketIsHeld bucketIsHeld, in Position position) =>
                {
                    bucket.WaterLevel += timeData.DeltaTime / configValues.WaterFillUpDuration;
                    if (Unity.Burst.CompilerServices.Hint.Unlikely(bucket.WaterLevel >= 1))
                    {
                        bucket.WaterLevel = 1f;
                    
                        // Set bucket tags:
                        Utils.StopFillingUpBucket(concurrentEcb, entityInQueryIndex, bucketEntity);

                        // Drop the bucket:
                        Utils.DropBucket(concurrentEcb, entityInQueryIndex, bucketIsHeld.WorkerHoldingThis, bucketEntity, position.Value);
                    }
                }).ScheduleParallel();


            var bucketEntities = QueryBucketEntities();
            var distanceToPickupBucketSqr = configValues.DistanceToPickupBucket * configValues.DistanceToPickupBucket;
            
            // Start filling up bucket:
            Entities
                .WithName("PickUpEmptyBucketsAtWaterSource")
                .WithBurst()
                .WithReadOnly(teamDatas)
                .WithReadOnly(bucketEntities)
                .WithReadOnly(bucketPositions)
                .WithDisposeOnCompletion(bucketPositions)
                .WithDisposeOnCompletion(bucketEntities)
                .WithAll<BucketFillersTag>()
                .WithNone<WorkerIsHoldingBucket>()
                .ForEach((Entity workerEntity, int entityInQueryIndex, ref Position workerPosition, in TeamId teamId) =>
                {
                    // Find water source.
                    var teamData = teamDatas[teamId.Id];
                    if (!teamData.IsValid) 
                        return;
                    
                    var waterSourcePosition = teamData.TargetWaterPos;

                    if (Utils.MoveToPosition(ref workerPosition, waterSourcePosition, configValues.WorkerSpeed * timeData.DeltaTime))
                    {
                        // Find NON-FULL buckets inside water source.
                        Utils.GetClosestBucket(waterSourcePosition, bucketPositions, out var sqrDistanceToBucket, out var closestBucketEntityIndex);

                        // Found a bucket, start carrying to team mate
                        if (sqrDistanceToBucket < distanceToPickupBucketSqr && closestBucketEntityIndex >= 0)
                        {
                            var bucketEntity = bucketEntities[closestBucketEntityIndex];
                            Utils.AddPickUpBucketRequest(concurrentEcb, entityInQueryIndex, workerEntity, bucketEntity, Utils.PickupRequestType.FillUp);
                        }
                    }

                    // Pick one up.
                }).ScheduleParallel();
        

            AddECBAsDependency();
        }
    }
}
