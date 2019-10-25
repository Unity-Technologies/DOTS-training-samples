using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Physics : IComponentData
{
    public float3 velocity;
    public float GravityStrength;
    public float3 angularVelocity;
    public bool flying;
}
