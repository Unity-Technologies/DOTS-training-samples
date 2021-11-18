using System;
using Unity.Entities;

namespace Dots
{
    [Serializable]
    public struct BeamComponent : IComponentData
    {
        public int beamIndex;
    }
}