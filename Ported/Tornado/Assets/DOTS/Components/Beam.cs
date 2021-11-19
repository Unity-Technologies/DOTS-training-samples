using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Dots
{
    public struct Beam : IComponentData
    {
        public int beamDataIndex;
    }

    public struct BeamData
    {
        public int p1;
        public int p2;

        public bool toBreakP1;
        public bool toBreakP2;

        public float3 newD;
        public float3 oldD;
        public float length;
    }
}

