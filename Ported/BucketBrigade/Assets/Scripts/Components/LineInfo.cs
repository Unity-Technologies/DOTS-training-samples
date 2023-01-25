using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct LineInfo : IComponentData
{
    public int index;
    public float3 position;
    public float3 previousPosition;
    public float3 nextPosition;
}

