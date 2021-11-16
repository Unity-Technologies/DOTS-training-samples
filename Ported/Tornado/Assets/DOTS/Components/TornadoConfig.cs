using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Dots
{
    public struct TornadoConfig : IComponentData
    {
        public bool simulate;
        public float spinRate;
        public float upwardSpeed;
        public float initRange;
        public float force;
        public float maxForceDist;
        public float height;
        public float upForce;
        public float inwardForce;
    }
}