using Unity.Entities;
using UnityEngine;

namespace src.DOTS.Components
{
    public struct PlatformQueue : IComponentData
    {
        public int platformIndex;
    }
}