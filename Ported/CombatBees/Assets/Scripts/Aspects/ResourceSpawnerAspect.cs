using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

readonly partial struct ResourceSpawnerAspect : IAspect
{
    readonly RefRO<ResourceSpawner> m_ResourceSpawner;

    public Entity ResourceSpawn => m_ResourceSpawner.ValueRO.ResourceSpawn;
    public Entity ResourcePrefab => m_ResourceSpawner.ValueRO.ResourcePrefab;
}
