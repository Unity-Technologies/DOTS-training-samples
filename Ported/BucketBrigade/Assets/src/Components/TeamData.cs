using System;
using Unity.Entities;
using Unity.Mathematics;

namespace src.Components
{
    /// <summary>
    ///     Added to a unique team entity, denotes this teams goals.
    /// </summary>
    public struct TeamData : IBufferElementData
    {
        public float2 TargetWaterPos;

        /// <summary>
        ///     Optimization: Fast lookup, should match <see cref="TargetFireCell" />!
        /// </summary>
        public float2 TargetFirePos;
        
        public int TargetFireCell;
    }
}
