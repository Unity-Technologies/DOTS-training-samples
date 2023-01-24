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
    public int DroneCost
    {
        get => m_silo.ValueRW.DroneCost;
        set => m_silo.ValueRW.DroneCost = value;
    }

    public byte HireType
    {
        get => m_silo.ValueRW.HireType;
        set => m_silo.ValueRW.HireType= value;
    }
}
