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
        /// <summary>
        ///     False if this TeamData has no valid target. Note (0, 0) is a valid position for the fire, so we only check for water here
        /// </summary>
        public bool IsValid => math.any(TargetWaterPos);

        public float2 MiddlePos => math.lerp(TargetFirePos, TargetWaterPos, 0.5f);

        public float2 TargetWaterPos;

        /// <summary>
        ///     Optimization: Fast lookup, should match <see cref="TargetFireCell" />!
        /// </summary>
        public float2 TargetFirePos;
        
        public int TargetFireCell;
    }
}
