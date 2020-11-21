using Unity.Mathematics;
using Unity.Entities;

/// <summary>
/// Component Data
/// </summary>
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

/// <summary>
/// Tag
/// </summary>
public struct Dead : IComponentData
{
}

/// <summary>
/// Reference
/// </summary>
public struct TargetBee : IComponentData
{
    public Entity beeRef;
}

public struct TargetResource : IComponentData
{
    public Entity resRef;
}
