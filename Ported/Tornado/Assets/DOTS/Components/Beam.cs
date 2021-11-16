using System;
using Unity.Entities;

namespace Dots
{
    [Serializable]
    public struct Beam : IComponentData
    {
        public Entity p1;
        public Entity p2;
    }
}
