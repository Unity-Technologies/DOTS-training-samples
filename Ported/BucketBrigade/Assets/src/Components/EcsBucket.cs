using System;
using Unity.Entities;

namespace src.Components
{
    /// <summary>
    /// </summary>
    [GenerateAuthoringComponent]
    public struct EcsBucket : IComponentData
    {
        /// <summary>
        ///     Normalized value. 0 = Empty. 1 = Full. Capacity defined in config.
        /// </summary>
        public float WaterLevel;
    }
}
