using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Dots
{
    public struct Anchor : IComponentData
    {
        public float3 position;
        public float3 oldPosition;
        public int neighborCount;
        public bool anchored;
    }
}

