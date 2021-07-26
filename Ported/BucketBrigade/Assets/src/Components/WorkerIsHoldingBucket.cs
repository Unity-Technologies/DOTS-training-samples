using System;
using Unity.Entities;

namespace src.Components
{
    /// <summary>
    ///     
    /// </summary>
    public struct WorkerIsHoldingBucket : IComponentData
    {
        public Entity Bucket;
    }
}