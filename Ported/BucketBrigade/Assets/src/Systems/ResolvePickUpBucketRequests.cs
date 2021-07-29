using System;
using src.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace src.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
    public class ResolvePickUpBucketRequests : SystemBase
    {
        EntityQuery m_Query;
        EntityQuery m_InvalidRequests;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_InvalidRequests = GetEntityQuery(ComponentType.ReadOnly<PickUpBucketRequest>(), ComponentType.ReadOnly<BucketIsHeld>());
        }

        protected override void OnUpdate()
        {
            if (! m_InvalidRequests.IsEmpty)
            {
                using var invalidEntities = m_InvalidRequests.ToEntityArray(Allocator.Temp);
                var errorString = "";
                for (var i = 0; i < invalidEntities.Length; i++) 
                    errorString += $"\n{invalidEntities[i]} '{EntityManager.GetName(invalidEntities[i])}'";
                Debug.LogError($"You added {invalidEntities.Length} ResolvePickUpBucketRequest[s] to [a] HELD bucket[s]! {errorString}");
                EntityManager.RemoveComponent<PickUpBucketRequest>(m_Query);
            }
            
            var ecb = new EntityCommandBuffer(Allocator.TempJob, PlaybackPolicy.SinglePlayback);
            var parallelEcb = ecb.AsParallelWriter();
            
            Entities.WithName("PickupBuckets").WithNone<BucketIsHeld>().WithBurst().WithStoreEntityQueryInField(ref m_Query).ForEach((Entity bucketEntity, int entityInQueryIndex, PickUpBucketRequest request) =>
            {
                // NW: We're using the quirk that the ECB will only allow 1 entity to ultimately set the
                // final request on a bucket to denote who actually gets to pickup the bucket:
                parallelEcb.AddComponent(entityInQueryIndex, bucketEntity, new BucketIsHeld
                {
                    WorkerHoldingThis = request.WorkerRequestingToPickupBucket,
                });
                parallelEcb.AddComponent(entityInQueryIndex,request.WorkerRequestingToPickupBucket, new WorkerIsHoldingBucket
                {
                    Bucket = bucketEntity,
                });

                switch (request.PickupRequestType)
                {
                    case Utils.PickupRequestType.FillUp:
                        parallelEcb.AddComponent<FillingUpBucketTag>(entityInQueryIndex, bucketEntity);
                        break;
                    case Utils.PickupRequestType.Carry:
                        // Do nothing.
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(request.PickupRequestType), request.PickupRequestType, nameof(ResolvePickUpBucketRequests));
                }
            }).ScheduleParallel();
            EntityManager.RemoveComponent<PickUpBucketRequest>(m_Query);
            
            // Playback resolutions.
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}
