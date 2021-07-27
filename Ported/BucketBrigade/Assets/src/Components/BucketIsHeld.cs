using System;
using Unity.Entities;

namespace src.Systems
{
    /// <summary>
    ///     Denotes a worker is holding the bucket.
    /// </summary>
    public struct BucketIsHeld : IComponentData
    {
        public Entity WorkerHoldingThis;
    }
}
