using System;
using Unity.Entities;

namespace src.Components
{
    /// <summary>
    ///     Optimization. Denotes that <see cref="EcsBucket.WaterLevel" /> is 1 (full).
    ///     Ensure you keep in sync with WaterLevel!
    ///     This would be added to a bucket.
    /// </summary>
    public struct FullBucketTag : IComponentData
    {
    }
}
