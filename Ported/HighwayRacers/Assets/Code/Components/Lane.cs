using Unity.Entities;
using UnityEngine;

namespace HighwayRacer
{
    [GenerateAuthoringComponent]
    public struct Lane : IComponentData
    {
        public byte Val;       // 0001 for right most lane
                               // 0010 for second lane from right
                               // 0100 for third land from right
                               // 1000 for fourth lane from right
                               
                               // 0011 for merging between two right most lanes
                               // 0110 for merging between two middle lanes
                               // 1100 for merging between two leftmost lanes
    }
}