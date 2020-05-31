using Unity.Entities;
using Unity.Mathematics;

public struct CanReservedTag : IComponentData
{
}

public struct CanReserveRequest : IComponentData
{
    public Entity canRef;
    public Entity armRef;
    public Entity rockRef;

    //used for tie breaking purposes
    public float3 armPos;
}

public struct CanTag : IComponentData
{
    
}

public struct CanInit : IComponentData
{
    public float3 initPos;
    public float3 initVel;
}