using System;
using Unity.Entities;

namespace Dots
{
    // This component is used to break chunks so that we can control their size
    [Serializable]
    public struct ChunkIndex : ISharedComponentData
    {
        public int index;
    }
}
