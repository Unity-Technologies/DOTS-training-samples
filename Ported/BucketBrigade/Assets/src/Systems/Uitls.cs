using src.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace src.Systems
{
    public static class Utils
    {
        /// <summary>
        /// Moves to target position
        /// </summary>
        /// <returns>
        /// True - we're at target position
        /// False - still moving
        /// </returns>
        public static bool MoveToPosition(ref Position pos, float2 targetPosition, float speed)
        {
            // TODO: Simplify redundant math.
            var deltaMovement = math.normalizesafe(targetPosition - pos.Value) * speed;
            var currentDistanceSq = math.distancesq(pos.Value, targetPosition);

            if (currentDistanceSq <= math.length(deltaMovement))
            {
                // Might make it look more obvious: pos.Value = targetPosition;
                return true;
            }
            else
            {
                pos.Value += deltaMovement;
                return false;
            }
        }

        public enum PickupRequestType
        {
            FillUp,
            Carry
        }
        public static void AddPickUpBucketRequest(EntityCommandBuffer.ParallelWriter ecb, int queryIndex, Entity workerEntity, Entity bucketEntity, PickupRequestType type)
        {
            ecb.AddComponent(queryIndex, bucketEntity, new PickUpBucketRequest
            {
                WorkerRequestingToPickupBucket = workerEntity,
                PickupRequestType = type,
            });
        }       
        

        public static void ThrowBucketAtFire(EntityCommandBuffer.ParallelWriter ecb, int queryIndex, Entity bucketEntity, float2 firePosition)
        {
            ecb.AddComponent(queryIndex, bucketEntity, new ThrowBucketAtFire() { firePosition = firePosition });
        }

        public static void DropBucket(EntityCommandBuffer.ParallelWriter ecb, int queryIndex, Entity workerEntity, Entity bucketEntity, float2 targetPos)
        {
            ecb.RemoveComponent<BucketIsHeld>(queryIndex, bucketEntity);
            ecb.RemoveComponent<WorkerIsHoldingBucket>(queryIndex, workerEntity);
            ecb.SetComponent(queryIndex, bucketEntity, new Position{ Value = targetPos});
        }

        public static Position GetClosestBucket(float2 position, NativeArray<Position> bucketPositions, out float closestSqrDistance, out int closestBucketEntityIndex)
        {
            var closestBucketPosition = new Position();
            closestSqrDistance = float.PositiveInfinity;
            closestBucketEntityIndex = -1;
            for (var i = 0; i < bucketPositions.Length; i++)
            {
                var bucketPosition = bucketPositions[i];
                var bucketSqrDistance = math.distancesq(position, bucketPosition.Value);
                if (bucketSqrDistance < closestSqrDistance)
                {
                    closestBucketPosition = bucketPosition;
                    closestSqrDistance = bucketSqrDistance;
                    closestBucketEntityIndex = i;
                }
            }
            return closestBucketPosition;
        }   
        
        /// <summary>
        /// Similar to <see cref="GetClosestBucket"/>, except we IGNORE buckets WITHIN our teams water position.
        /// </summary>
        public static Position GetClosestBucketOutsideTeamWaterSource(float2 ourPos, float2 teamWaterPos, NativeArray<Position> bucketPositions, out float closestSqrDistance, out int closestBucketEntityIndex, float distanceToPickupBucketSqr)
        {
            var closestBucketPosition = new Position();
            closestSqrDistance = float.PositiveInfinity;
            closestBucketEntityIndex = -1;
            for (var i = 0; i < bucketPositions.Length; i++)
            {
                var bucketPosition = bucketPositions[i];
                var bucketSqrDistanceToUs = math.distancesq(ourPos, bucketPosition.Value);
                var bucketSqrDistanceToWater = math.distancesq(teamWaterPos, bucketPosition.Value);
                var ignoreAsAlreadyInWaterZone = bucketSqrDistanceToWater > distanceToPickupBucketSqr;
                if (bucketSqrDistanceToUs < closestSqrDistance && ignoreAsAlreadyInWaterZone)
                {
                    closestBucketPosition = bucketPosition;
                    closestSqrDistance = bucketSqrDistanceToUs;
                    closestBucketEntityIndex = i;
                }
            }
            return closestBucketPosition;
        }

        public static float3 To3D(float2 pos2D) => new float3(pos2D.x, 0, pos2D.y);
        public static float2 To2D(float3 pos3D) => pos3D.xz;
        
        public static void StopFillingUpBucket(EntityCommandBuffer.ParallelWriter concurrentEcb, int entityInQueryIndex, Entity bucketEntity)
        {
            concurrentEcb.RemoveComponent<FillingUpBucketTag>(entityInQueryIndex, bucketEntity);
            concurrentEcb.AddComponent<FullBucketTag>(entityInQueryIndex, bucketEntity);
        }
    }
}