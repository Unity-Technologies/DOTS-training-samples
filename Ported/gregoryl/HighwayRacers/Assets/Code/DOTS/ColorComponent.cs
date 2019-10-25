using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace HighwayRacers
{
    public struct ColorComponent : IComponentData
    {
        public float4 Value;
    }
}
