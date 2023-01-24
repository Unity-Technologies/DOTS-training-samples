using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

// Instead of directly accessing the Silo component, we are creating an aspect.
// Aspects allows you to provide a customized API for accessing your components.
readonly partial struct SiloAspect : IAspect
{
    readonly RefRO<Config> m_config;
    readonly RefRO<Silo> m_silo;

    public Entity FarmerPrefab => m_config.ValueRO.FarmerPrefab;
    public Entity DronePrefab => m_config.ValueRO.DronePrefab;
    public Entity FarmerSpawn => m_silo.ValueRO.FarmerSpawn;
    public Entity DroneSpawn => m_silo.ValueRO.DroneSpawn;
}
