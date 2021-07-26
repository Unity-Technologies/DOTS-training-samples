using System;
using Unity.Entities;
using Unity.Mathematics;

namespace src.Components
{
    /// <summary>
    ///     
    /// </summary>
    public struct TeamData : IComponentData
    {
        public float2 TargetWaterPos;
        public int TargetFileCell;
    }
}