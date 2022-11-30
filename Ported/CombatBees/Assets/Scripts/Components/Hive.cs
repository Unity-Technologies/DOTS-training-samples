using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Hive : IComponentData
{
    public float4 color;
    public float3 boundsExtents;
    public float3 boundsPosition;
}

public struct EnemyBees : IBufferElementData
{
    public Entity enemy;
    public float3 enemyPosition;
}

public struct AvailableResources : IBufferElementData
{
    public Entity resource;
    public float3 resourcePosition;
}
