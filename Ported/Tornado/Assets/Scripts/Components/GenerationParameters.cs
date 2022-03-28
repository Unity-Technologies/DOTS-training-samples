using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct GenerationParameters : IComponentData
    {
        public int particleCount;
        public float3 minParticleSpawnPosition;
        public float3 maxParticleSpawnPosition;
        public Entity particlePrefab;
        public float minParticleScale;
        public float maxParticleScale;

        // .. more will come

        public int cubeSize;

        public Entity barPrefab;
    }
}