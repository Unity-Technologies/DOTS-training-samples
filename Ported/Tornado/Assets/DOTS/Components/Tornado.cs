using Unity.Entities;
using Unity.Mathematics;

namespace Dots
{
    public struct Tornado : IComponentData
    {
        public float3 initialPosition;
        public float3 position;
        public float spinRate;
        public float upwardSpeed;
        public float force;
        public float maxForceDist;
        public float height;
        public float upForce;
        public float inwardForce;
        public float rotationModulation;
    }
}