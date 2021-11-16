using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Dots
{
    [Serializable]
    public struct Beam : IComponentData
    {
        public Entity p1;
        public Entity p2;
        public float3 oldDelta;
    }
}
