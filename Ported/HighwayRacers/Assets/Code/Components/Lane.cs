using Unity.Entities;
using UnityEngine;

namespace HighwayRacer
{
    [GenerateAuthoringComponent]
    public struct Lane : IComponentData
    {
        public byte Val;
    }
}