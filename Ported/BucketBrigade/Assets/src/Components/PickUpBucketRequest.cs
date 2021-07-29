using System;
using Unity.Entities;

namespace src.Systems
{
    /// <summary>
    ///     Added to a Bucket.
    ///     Denotes that a worker WANTS to pickup this bucket.
    ///     This solves Bursted race-condition where multiple workers attempt to pickup the same bucket.
    /// </summary>
    public struct PickUpBucketRequest : IComponentData
    {
        public Entity WorkerRequestingToPickupBucket;
        public Utils.PickupRequestType PickupRequestType;
    }
}
