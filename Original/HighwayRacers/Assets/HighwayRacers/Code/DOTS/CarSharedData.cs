using System;
using Unity.Entities;
using UnityEngine;

namespace HighwayRacers
{
    // TODO this should only be stored once somewhere
    [Serializable]
    public struct CarSharedData : IComponentData
    {
        public float distanceToFront;
        public float distanceToBack;
        public Color defaultColor;
        public Color maxSpeedColor;
        public Color minSpeedColor;
    }
}
