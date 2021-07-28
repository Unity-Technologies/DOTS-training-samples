using src.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

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
            var deltaMovement = math.normalizesafe(targetPosition - pos.Value) * speed;
            var currentDistanceSq = math.distancesq(pos.Value, targetPosition);

            if (currentDistanceSq <= math.length(deltaMovement))
            {
                pos.Value = targetPosition;

                return true;
            }
            else
            {
                pos.Value += deltaMovement;
                return false;
            }
        }

        public static void PickUpBucket(EntityCommandBuffer.ParallelWriter ecb, int queryIndex, Entity workerEntity, Entity bucketEntity)
        {
            ecb.AddComponent(queryIndex, bucketEntity, new BucketIsHeld
            {
                WorkerHoldingThis = workerEntity,
            });
            ecb.AddComponent(queryIndex, workerEntity, new WorkerIsHoldingBucket
            {
                Bucket = bucketEntity,
            });
        }

        public static void DropBucket(EntityCommandBuffer.ParallelWriter ecb, int queryIndex, Entity workerEntity, in WorkerIsHoldingBucket isHoldingBucket)
        {
            ecb.RemoveComponent<BucketIsHeld>(queryIndex, isHoldingBucket.Bucket);
            ecb.RemoveComponent<WorkerIsHoldingBucket>(queryIndex, workerEntity);
        }

        public static Position GetClosestBucket(Position myPosition, NativeArray<Position> bucketPositions, out float closestSqrDistance, out int closestBucketEntityIndex)
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