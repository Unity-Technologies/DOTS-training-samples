using System;
using Unity.Entities;
using Unity.Mathematics;

namespace src
{
    [Serializable]
    [InternalBufferCapacity(64)]
    public struct LineSegmentBufferElement : IBufferElementData
    {
        public float3 p0;
        public float3 p1;
        public float3 p2;
        public float3 p3;
    }
}