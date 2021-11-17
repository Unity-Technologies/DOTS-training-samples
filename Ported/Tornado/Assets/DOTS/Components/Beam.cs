using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Dots
{
    public struct Beam : IComponentData
    {
        public Entity p1;
        public Entity p2;

        public float3 newD;
        public float3 oldD;
        public float length;
    }
}

