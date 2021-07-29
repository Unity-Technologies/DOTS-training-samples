using System;
using src.Components;
using Unity.Entities;
using UnityEngine;

namespace src.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
    public class ResolvePickUpBucketRequests : SystemBase
    {
        EntityQuery m_Query;

        protected override void OnUpdate()
        {
            Entities.WithName(nameof(ResolvePickUpBucketRequests)).WithStructuralChanges().WithStoreEntityQueryInField(ref m_Query).ForEach((Entity bucketEntity, PickUpBucketRequest request) =>
            {
                if (EntityManager.HasComponent<BucketIsHeld>(bucketEntity))
                {
                    // Some other Entity has beaten this entity to it!
                    Debug.LogError($"Looks like you added a PickupBucketRequest to a HELD bucket: {bucketEntity}. Request type: {request.PickupRequestType} with worker: {request.WorkerRequestingToPickupBucket} '{EntityManager.GetName(request.WorkerRequestingToPickupBucket)}'");
                    return;
                }

                // NW: We're using the quirk that the ECB will only allow 1 entity to ultimately set the
                // final request on a bucket to denote who actually gets to pickup the bucket:
                EntityManager.AddComponentData(bucketEntity, new BucketIsHeld
                {
                    WorkerHoldingThis = request.WorkerRequestingToPickupBucket,
                });
                EntityManager.AddComponentData(request.WorkerRequestingToPickupBucket, new WorkerIsHoldingBucket
                {
                    Bucket = bucketEntity,
                });

                switch (request.PickupRequestType)
                {
                    case Utils.PickupRequestType.FillUp:
                        EntityManager.AddComponent<FillingUpBucketTag>(bucketEntity);
                        break;
                    case Utils.PickupRequestType.Carry:
                        // Do nothing.
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(request.PickupRequestType), request.PickupRequestType, nameof(ResolvePickUpBucketRequests));
                }
            }).Run();
            EntityManager.RemoveComponent<PickUpBucketRequest>(m_Query);
        }
    }
}
