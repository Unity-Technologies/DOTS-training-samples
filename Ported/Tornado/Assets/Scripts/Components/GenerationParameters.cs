using Unity.Collections;
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
        public float minColorMultiplier;
        public float maxColorMultiplier;

        public int citySize;
        public int groundDetails;
        public int buildings;
        public int buildingsPerIsland;

        public int spawnMapH;
        public int spawnMapW;

        public Entity barPrefab;
    }
}