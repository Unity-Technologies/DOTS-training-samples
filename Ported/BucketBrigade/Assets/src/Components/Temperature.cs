using System;
using Unity.Entities;

namespace src.Components
{
    /// <summary>
    ///     Denotes fire intensity. 0 to 1.
    /// </summary>
    [InternalBufferCapacity(0)]
    public struct Temperature : IBufferElementData
    {
        public float Intensity;

        public bool IsOnFire => Intensity > 0;
    }
}