using Systems.Particles;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Components
{
    public struct BeeParticleComponent : IComponentData
    {
        public ParticleType type;
        public float3 position;
        public float3 velocity;
        public float3 size;
        public float life;
        public float lifeDuration;
        public float4 color;
        public bool stuck;
    }
}