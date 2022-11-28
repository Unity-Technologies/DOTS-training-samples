using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Hive : IComponentData
{
    public float4 color;
    public float3 boundsExtents;
    public float3 boundsPosition;
    public Entity beePrefab;
    public int startBeeCount;
}
