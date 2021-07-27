﻿using System;
using Unity.Entities;
using Unity.Mathematics;

namespace src.Components
{
    /// <summary>
    ///     Added to a unique team entity, denotes this teams goals.
    /// </summary>
    [GenerateAuthoringComponent]
    public struct TeamData : IComponentData
    {
        public float2 TargetWaterPos;
        public int TargetFileCell;
    }
}