using System;
using src.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace src.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class BucketThrowerUpdate : BucketWorkerUpdateBase
    {
        protected override QueryBuckets WhichBucketsToQuery { get => QueryBuckets.Full; }

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<TeamData>();
            RequireSingletonForUpdate<Temperature>();
        }

        protected override void OnUpdate()
        {
            var teamContainerEntity = GetSingletonEntity<TeamData>();
            var teamDatas = EntityManager.GetBuffer<TeamData>(teamContainerEntity);
        
            var configValues = GetSingleton<FireSimConfigValues>();
            var timeData = Time;
            var bucketPositions = QueryBucketPositions();
            var bucketEntities = QueryBucketEntities();
            var distanceToPickupBucketSqr = configValues.DistanceToPickupBucket * configValues.DistanceToPickupBucket;
            var concurrentEcb = CreateECBParallerWriter();

            Entities.WithBurst()
                .WithName("BucketThrowerMovesIntoPositionAndPickBucket")
                .WithReadOnly(teamDatas)
                .WithReadOnly(bucketPositions)
                .WithReadOnly(bucketEntities)
                .WithDisposeOnCompletion(bucketPositions)
                .WithDisposeOnCompletion(bucketEntities)
                .WithAll<BucketThrowerTag>()
                .ForEach((int entityInQueryIndex, Entity workerEntity, ref Position pos, in TeamId ourTeamId, in TeamPosition teamPosition) =>
                {
                    var teamData = teamDatas[ourTeamId.Id];
                    if (! teamData.IsValid) return;
                    
                    var firePosition = teamData.TargetFirePos;

                    if (Utils.MoveToPosition(ref pos, firePosition, timeData.DeltaTime * configValues.WorkerSpeed) && bucketPositions.Length > 0)
                    {
                        Utils.GetClosestBucket(pos.Value, bucketPositions, out var sqrDistanceToBucket, out var closestBucketEntityIndex);

                        if (sqrDistanceToBucket < distanceToPickupBucketSqr)
                            Utils.ThrowBucketAtFire(concurrentEcb, entityInQueryIndex, bucketEntities[closestBucketEntityIndex], firePosition);
                    }
                }).ScheduleParallel();
            
            JobHandle.ScheduleBatchedJobs();
            
            AddECBAsDependency();
        }

    }
}
