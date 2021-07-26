using System;
using Unity.Entities;

namespace src.Components
{
    /// <summary>
    ///     Denotes that this is a water tile, allows <see cref="BucketTag"/> to be filled up.
    /// </summary>
    public struct WaterTag : IComponentData
    {
    }
}