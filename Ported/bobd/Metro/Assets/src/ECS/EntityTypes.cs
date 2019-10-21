using System;
using Unity.Entities;
using Unity.Mathematics;

namespace MetroECS
{
    struct Position2D : IComponentData
    {
        public float2 Value;
    }
    
    struct Speed2D : IComponentData
    {
        public float2 Value;
    }

    struct Acceleration2D : IComponentData
    {
        public float2 Value;
    }
}