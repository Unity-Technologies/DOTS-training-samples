using System;
using Unity.Entities;

namespace Dots
{
    public struct Debris : IComponentData
    {
        public Entity tornado;
        public float radiusMult;
    }
}

