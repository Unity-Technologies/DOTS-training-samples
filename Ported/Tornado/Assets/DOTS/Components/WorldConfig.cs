using System;
using Unity.Entities;

namespace Dots
{
    [Serializable]
    public struct WorldConfig : IComponentData
    {
        public float expForce;
        public float breakResistance;
        public float damping;
        public float friction;
    }
}
