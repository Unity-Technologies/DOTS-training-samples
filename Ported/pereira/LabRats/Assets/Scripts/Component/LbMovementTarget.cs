using System;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Movement target
/// </summary>
[Serializable]
public struct LbMovementTarget : IComponentData
{
    public float3 From;
    public float3 To;
}