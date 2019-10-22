using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct IsReserved : IComponentData
{
    public bool Value;
}

public struct AngularVelocity : IComponentData
{
    public float3 Value;
}

