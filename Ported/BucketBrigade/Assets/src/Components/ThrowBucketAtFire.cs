using System;
using Unity.Entities;
using Unity.Mathematics;

namespace src.Components
{
    public struct ThrowBucketAtFire : IComponentData
    {
        public float2 firePosition;
    }
}