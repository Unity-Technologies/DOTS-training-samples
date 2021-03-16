using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct ThrowVelocity : IComponentData
{
    public float3 Value;
}
