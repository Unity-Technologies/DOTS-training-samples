using Unity.Entities;
using Unity.Mathematics;

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
        public float rotationModulation;
        public float3 initialPosition;
    }
}