using System;
using Unity.Entities;

namespace src.Systems
{
    /// <summary>
    ///     Added to a Bucket.
    ///     Denotes that a worker is holding this bucket.
    /// </summary>
    public struct BucketIsHeld : IComponentData
    {
        // todo - Work out if we even need this data. Could be a tag component!
        public Entity WorkerHoldingThis;
    }
}
