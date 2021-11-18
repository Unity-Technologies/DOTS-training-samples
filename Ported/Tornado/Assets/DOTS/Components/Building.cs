using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Dots
{
    [Serializable]
    public struct Building : IComponentData
    {
        public int4 index;
    }
}
