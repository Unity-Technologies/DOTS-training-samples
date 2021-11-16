using System;
using Unity.Entities;

namespace Dots
{
    [Serializable]
    public struct Anchor : IComponentData
    {
        public int NeighborCount;
    }
}
