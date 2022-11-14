using Components;
using Unity.Entities;
using Unity.Mathematics;

namespace Aspects
{
    public readonly partial struct BeeConfigAspect
    {
        private readonly RefRO<BeeConfig> _config;
        private readonly RefRW<Random> _random;

        public Entity BeePrefab => _config.ValueRO.BeePrefab;
        public int BeesToSpawn => _config.ValueRO.BeesToSpawn;
        
        
        public float3 GetRandomPosition(float3 minBounds, float3 maxBounds)
        {
            float3 randomPosition;
            randomPosition = _random.ValueRW.NextFloat3(minBounds, maxBounds);
            return randomPosition;
        }
    }
}