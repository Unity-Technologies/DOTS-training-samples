using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct GenerationParameters : IComponentData
    {
        public float3 minParticleSpawnPosition;
        public float3 maxParticleSpawnPosition;
        // .. more will come
    }
}