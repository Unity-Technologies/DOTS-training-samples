using System;
using src.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace src.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class BucketThrowerUpdate : BucketWorkerUpdateBase
    {
        protected override QueryBuckets WhichBucketsToQuery { get => QueryBuckets.Full; }

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
                    // TODO: Get Fire position
                    var firePosition = new float2(100, 100);

                    if (Utils.MoveToPosition(ref pos, firePosition, timeData.DeltaTime * configValues.WorkerSpeed) && bucketPositions.Length > 0)
                    {
                        Utils.GetClosestBucket(pos, bucketPositions, out var sqrDistanceToBucket, out var closestBucketEntityIndex);

                        if (sqrDistanceToBucket < distanceToPickupBucketSqr)
                            Utils.ThrowBucketAtFire(concurrentEcb, entityInQueryIndex, bucketEntities[closestBucketEntityIndex], firePosition);
                    }
                }).ScheduleParallel();

            AddECBAsDependency();
        }

    }
}
