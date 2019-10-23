using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace HighwayRacers
{
    struct CarColor : IComponentData
    {
        public float4 Value;
    }
}
