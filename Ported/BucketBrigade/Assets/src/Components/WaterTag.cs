using System;
using Unity.Entities;

namespace src.Components
{
    /// <summary>
    ///     Denotes that this is a water tile, allows <see cref="EcsBucket"/> to be filled up.
    /// </summary>
    [GenerateAuthoringComponent]
    public struct WaterTag : IComponentData
    {
        public float CurrentWaterVolume;
        public float MaximumWaterVolume;
    }
}