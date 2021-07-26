using System;
using Unity.Entities;

namespace src.Components
{
    /// <summary>
    ///     Denotes how far the bucket is through the transition to move it to another worker.
    /// </summary>
    public struct BucketHandoverState : IComponentData
    {
        public float NormalizedValue;
    }
}