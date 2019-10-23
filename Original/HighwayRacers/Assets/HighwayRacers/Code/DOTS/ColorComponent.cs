using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace HighwayRacers
{
    struct ColorComponent : IComponentData
    {
        public float4 Value;
    }
}
