using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;

using src.Components;

namespace src.Systems
{
    public class OmniWorkerSystem: SystemBase
    {
        EntityQuery m_NotFullBucketsOnFloorQuery;
        EndSimulationEntityCommandBufferSystem m_EndSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<FireSimConfigValues>();

            m_NotFullBucketsOnFloorQuery = GetEntityQuery(ComponentType.ReadOnly<EcsBucket>(), ComponentType.Exclude<BucketIsHeld>(), ComponentType.Exclude<PickUpBucketRequest>(), ComponentType.Exclude<FillingUpBucketTag>(), ComponentType.Exclude<FullBucketTag>(), ComponentType.ReadOnly<Position>());
            m_EndSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var time = Time;
            var configValues = GetSingleton<FireSimConfigValues>();
            
            var ecb = m_EndSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            var concurrentEcb = ecb.AsParallelWriter();

            // Pickup a bucket. Omni workers never drop.
            if (!m_NotFullBucketsOnFloorQuery.IsEmpty)
            {
                var distanceToPickupBucketSqr = configValues.DistanceToPickupBucket * configValues.DistanceToPickupBucket;
                var bucketEntities = m_NotFullBucketsOnFloorQuery.ToEntityArray(Allocator.TempJob);
                var bucketPositions = m_NotFullBucketsOnFloorQuery.ToComponentDataArray<Position>(Allocator.TempJob);

                Entities.WithBurst()
                    .WithName("OmniMoveTowardsAndPickupBucket")
                    .WithReadOnly(bucketPositions)
                    .WithReadOnly(bucketEntities)
                    .WithDisposeOnCompletion(bucketPositions)
                    .WithDisposeOnCompletion(bucketEntities)
                    .WithAll<OmniWorkerTag>()
                    .WithNone<WorkerIsHoldingBucket>()
                    .ForEach((int entityInQueryIndex, Entity workerEntity, ref Position pos) =>
                    {
                        var closestBucketPosition = Utils.GetClosestBucketOutsideTeamWaterSource(pos.Value, pos.Value, bucketPositions, out _, out var closestBucketEntityIndex, distanceToPickupBucketSqr);

                        if (closestBucketEntityIndex >= 0)
                        {
                            if (Utils.MoveToPosition(ref pos, closestBucketPosition.Value, configValues.WorkerSpeed * time.DeltaTime))
                            {
                                Utils.AddPickUpBucketRequest(concurrentEcb, entityInQueryIndex, workerEntity, bucketEntities[closestBucketEntityIndex], Utils.PickupRequestType.Carry);
                            }
                        }
                        // Else idle as no buckets left.
                    }).ScheduleParallel();
            }

            Entities.WithBurst()
                    .WithName("OmniMoveTowardsClosestWaterSource")
                    .WithAll<OmniWorkerTag>()
                    .WithAll<WorkerIsHoldingBucket>()
                    .ForEach((int entityInQueryIndex, Entity workerEntity, ref Position pos) =>
                    {
                        // TODO: Move to nearest water source
                    }).ScheduleParallel();

            // TODO: Fill up bucket
            // TODO: Move to Fire
        }
    }
}