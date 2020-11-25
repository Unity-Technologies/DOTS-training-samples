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
    public float size;
}

/*
public struct StackIndex : IComponentData
{
    public float stIndex;
}

public struct GridX : IComponentData
{
    public float gridX;
}

public struct GridY : IComponentData
{
    public float gridY;
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
