using System;
using Unity.Entities;
using Unity.Mathematics;

public struct MovementComponent : IComponentData
{
    public float3 targetPosition;
    public float3 direction;
}

