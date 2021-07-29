using System;
using Unity.Entities;
using Unity.Mathematics;

namespace src.Components
{
    /// <summary>
    ///     Added to a Bucket entity.
    /// </summary>
    public struct ThrowBucketAtFire : IComponentData
    {
        public float2 firePosition;
    }
}