using Unity.Mathematics;
using Unity.Entities;


///////////////////////////////////////////////////////
/// <summary>
/// Component Data
/// </summary>
//////////////////////////////////////////////////////
public struct Velocity : IComponentData
{
    public float3 vel;
}

public struct DeathTimer : IComponentData
{
    public float dTimer;
}

/*
public struct SmoothPosition : IComponentData
{
    public float3 smPos;
}

public struct SmoothDirection : IComponentData
{
    public float3 smDir;
}


public struct Size : IComponentData
{
    public float value;
}
*/

///////////////////////////////////////////////////////
/// <summary>
/// Tag
/// </summary>
/// ///////////////////////////////////////////////////////
public struct Dead : IComponentData
{
}

public struct IsAttacking : IComponentData
{
}

public struct Stacked : IComponentData
{
}

public struct IsHoldingResource : IComponentData
{
}


///////////////////////////////////////////////////////
/// <summary>
/// Reference
/// </summary>
/// ///////////////////////////////////////////////////////
public struct TargetBee : IComponentData
{
    public Entity bee;
}

public struct TargetResource : IComponentData
{
    public Entity res;
}

public struct HolderBee : IComponentData
{
    public Entity holder;
}
