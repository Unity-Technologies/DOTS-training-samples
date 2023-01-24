using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

// Instead of directly accessing the Silo component, we are creating an aspect.
// Aspects allows you to provide a customized API for accessing your components.
readonly partial struct SiloAspect : IAspect
{
    public readonly RefRW<Silo> m_silo;
    public readonly TransformAspect Transform;

    public Entity FarmerSpawn => m_silo.ValueRO.FarmerSpawn;
    public Entity DroneSpawn => m_silo.ValueRO.DroneSpawn;

    public int Cash
    {
        get => m_silo.ValueRW.Cash ;
        set => m_silo.ValueRW.Cash = value;
    }
    public int FarmerCost
    {
        get => m_silo.ValueRW.FarmerCost;
        set => m_silo.ValueRW.FarmerCost = value;
    }

}
