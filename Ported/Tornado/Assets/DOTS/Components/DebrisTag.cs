using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Dots
{
    [Serializable]
    public struct DebrisTag : IComponentData
    {
        public Entity tornado;
        public float radiusMult;
    }
}
