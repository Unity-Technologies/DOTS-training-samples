using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Dots
{
    [Serializable]
    public struct BeamData : IComponentData
    {
        public float4x4 matrix;
        public float3 p1;
        public float3 p2;
    }
}
