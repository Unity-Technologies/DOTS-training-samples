using System;
using Unity.Entities;

namespace Dots
{
    [Serializable]
    public struct TornadoFader : IComponentData
    {
        public float value;
    }
}
