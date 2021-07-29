using System;
using Unity.Entities;

namespace src.Components
{
    /// <summary>
    ///     Added to the WORKER (who is holding a BUCKET).
    /// </summary>
    public struct WorkerIsHoldingBucket : IComponentData
    {
        public Entity Bucket;
    }
}