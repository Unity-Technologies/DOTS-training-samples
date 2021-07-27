using System;
using Unity.Entities;

namespace src.Systems
{
    /// <summary>
    ///     Denotes a worker is holding the bucket.
    /// </summary>
    public struct BucketIsHeld : IComponentData
    {
        // todo - Work out if we even need this data. Could be a tag component!
        public Entity WorkerHoldingThis;
    }
}
