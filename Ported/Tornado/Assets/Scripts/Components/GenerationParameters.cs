using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct GenerationParameters : IComponentData
    {
        public float3 minParticleSpawnPosition;
        public float3 maxParticleSpawnPosition;
        // .. more will come

        public int cubeSize;

        public Entity barPrefab;
    }
}