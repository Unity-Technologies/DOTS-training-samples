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
        EntityQuery m_BucketsOnFloorQuery;
        EndSimulationEntityCommandBufferSystem m_EndSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<FireSimConfigValues>();
            m_BucketsOnFloorQuery = GetEntityQuery(ComponentType.ReadOnly<EcsBucket>(), ComponentType.Exclude<BucketIsHeld>(), ComponentType.ReadOnly<Position>());
            m_EndSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var configValues = GetSingleton<FireSimConfigValues>();
            
            // Anyone with a BucketFetcherTag who hasn't got a bucket should find a bucket and move towards it.
            if (! m_BucketsOnFloorQuery.IsEmpty)
            {
                var distanceToPickupBucketSqr = configValues.DistanceToPickupBucket * configValues.DistanceToPickupBucket;
                var bucketEntities = m_BucketsOnFloorQuery.ToEntityArray(Allocator.TempJob);
                var bucketPositions = m_BucketsOnFloorQuery.ToComponentDataArray<Position>(Allocator.TempJob);

                var ecb = m_EndSimulationEntityCommandBufferSystem.CreateCommandBuffer();
                var concurrentEcb = ecb.AsParallelWriter();
                
                var timeData = Time;
                Entities.WithBurst()
                    .WithName("MoveTowardsAndPickupBuckets")
                    .WithReadOnly(bucketPositions)
                    .WithReadOnly(bucketEntities)
                    .WithDisposeOnCompletion(bucketPositions)
                    .WithDisposeOnCompletion(bucketEntities)
                    .WithAll<BucketFetcherWorkerTag>()
                    .WithNone<WorkerIsHoldingBucket>()
                    .ForEach((int entityInQueryIndex, Entity workerEntity, ref Position pos) =>
                    {
                        var closestBucketPosition = GetClosestBucket(pos, bucketPositions, out var sqrDistanceToBucket, out var closestBucketEntityIndex);
                        var deltaToBucket = closestBucketPosition.Value - pos.Value;
                        if (sqrDistanceToBucket < distanceToPickupBucketSqr)
                        {
                            var closestBucketEntity = bucketEntities[closestBucketEntityIndex];
                            concurrentEcb.AddComponent(entityInQueryIndex, closestBucketEntity, new BucketIsHeld
                            {
                                WorkerHoldingThis = workerEntity,
                            });
                            concurrentEcb.AddComponent(entityInQueryIndex, closestBucketEntity, new WorkerIsHoldingBucket()
                            {
                                Bucket = closestBucketEntity,
                            });
                        }
                        else
                        {
                            pos.Value += math.normalizesafe(deltaToBucket) * timeData.DeltaTime * configValues.WorkerSpeed;
                        }
                    }).ScheduleParallel();

                m_EndSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
            }

            // Anyone with a BucketFetcherTag who IS holding a bucket should move towards THEIR OWN TEAM POS, and drop the bucket for that team to use.
        }

        static Position GetClosestBucket(Position myPosition, NativeArray<Position> bucketPositions, out float closestSqrDistance, out int closestBucketEntityIndex)
        {     
            var closestBucketPosition = new Position();
            closestSqrDistance = float.PositiveInfinity;
            closestBucketEntityIndex = -1;
            for (var i = 0; i < bucketPositions.Length; i++)
            {
                var bucketPosition = bucketPositions[i];
                var bucketSqrDistance = math.distancesq(myPosition.Value, bucketPosition.Value);
                if (bucketSqrDistance < closestSqrDistance)
                {
                    closestBucketPosition = bucketPosition;
                    closestSqrDistance = bucketSqrDistance;
                    closestBucketEntityIndex = i;
                }
            }
            return closestBucketPosition;
        }
    }
}
