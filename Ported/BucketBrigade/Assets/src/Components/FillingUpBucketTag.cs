using System;
using src.Systems;
using Unity.Entities;

namespace src.Components
{
    /// <summary>
    ///     Denotes that the <see cref="BucketIsHeld.WorkerHoldingThis"/> worker is filling us up.
    ///     This would be added to a bucket.
    /// </summary>
    public struct FillingUpBucketTag : IComponentData
    {
    }
}
