using Unity.Entities;
using Unity.Mathematics;


public struct RockReservedCan : IComponentData
{
    public Entity Value;

    public static implicit operator RockReservedCan(Entity can) => new RockReservedCan
    {
        Value = can
    };

    public static implicit operator Entity(RockReservedCan c) => c.Value;
}

struct RockGrabbedTag : IComponentData
{
}


public struct RockReservedTag : IComponentData
{
}

public struct RockReserveRequest : IComponentData
{
    public Entity armRef;
    public Entity rockRef;

    //used for tie breaking purposes
    public float3 armPos;
}

public struct RockRadiusComponentData : IComponentData
{
    public float Value;

    public static implicit operator RockRadiusComponentData(float rad) => new RockRadiusComponentData()
    {
        Value = rad
    };

    public static implicit operator float(RockRadiusComponentData c) => c.Value;
}

public struct RockCollisionRNG : IComponentData
{
    public Random Value;
}