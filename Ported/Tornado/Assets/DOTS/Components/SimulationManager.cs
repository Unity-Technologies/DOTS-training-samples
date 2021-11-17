using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Dots
{
    public struct SimulationManager : IComponentData
    {
        public float expForce;
        public float breakResistance;
        public float damping;
        public float friction;
    }
}

