using Unity.Entities;
using UnityEngine;

namespace HighwayRacers
{
    struct CarColor : IComponentData
    {
        public Color Value;
    }
}
